using EasyNetQ;
using EasyNetQ.Consumer;
using EasyNetQ.SystemMessages;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using SWTI.EasyNetQ.Hosepipe;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SWTI.RabbitMQCore
{
    public class ErrorRetry : IErrorRetry
    {
        private readonly ILogger<ErrorRetry> _logger;
        private readonly IErrorMessageSerializer _errorMessageSerializer;
        private readonly QueueParameters _queueParameters;
        private readonly ISerializer _serializer;
        private readonly string _retryCountHeaderKey = "retry-count";
        private readonly IBus _bus;
        private readonly string _deadQueue = Const.DeadName;
        private readonly string _errorQueue = Const.ErrorName;

        public ErrorRetry(
             ILogger<ErrorRetry> logger,
            IErrorMessageSerializer errorMessageSerializer,
            QueueParameters queueParameters,
            ISerializer serializer,
            IBus bus
            )
        {
            _logger = logger;
            _errorMessageSerializer = errorMessageSerializer;
            _queueParameters = queueParameters;
            _serializer = serializer;
            _bus = bus;
        }

        private IEnumerable<HosepipeMessage> GetAllErrorQueue()
        {
            List<HosepipeMessage> list = new List<HosepipeMessage>();
            foreach (var item in Storage.QueueNameStore)
            {
                list.AddRange(GetMessagesFromQueue(item + _errorQueue));
            }
            return list;
        }

        private IEnumerable<HosepipeMessage> GetMessagesFromQueue(string queueName)
        {
            using var connection = HosepipeConnection.FromParameters(_queueParameters);
            using var channel = connection.CreateModel();

            _logger.LogDebug("GetMessagesFromQueue NumberOfMessagesToRetrieve: {0}", _queueParameters.NumberOfMessagesToRetrieve);
            channel.ConfirmSelect();
            var count = 0;
            while (count++ < _queueParameters.NumberOfMessagesToRetrieve)
            {
                var basicGetResult = channel.BasicGet(queueName, _queueParameters.Purge);
                if (basicGetResult == null) break; // no more messages on the queue
                var properties = new MessageProperties();
                properties.CopyFrom(basicGetResult.BasicProperties);

                var info = new MessageReceivedInfo(
                    "hosepipe",
                    basicGetResult.DeliveryTag,
                    basicGetResult.Redelivered,
                    basicGetResult.Exchange,
                    basicGetResult.RoutingKey,
                    queueName);
                var hosepipeMessage = new HosepipeMessage(_errorMessageSerializer.Serialize(basicGetResult.Body.ToArray()), properties, info);
                _logger.LogDebug("GetMessagesFromQueue {0} {1} {2}", basicGetResult.Exchange, hosepipeMessage.Body, queueName);
                yield return hosepipeMessage;
            }
        }

        public async Task RetryErrorsAsync(IEnumerable<HosepipeMessage> rawErrorMessages, CancellationToken cancellationToken)
        {
            List<Task> tasks = new List<Task>();
            foreach (var rawErrorMessage in rawErrorMessages)
            {
                var error = (Error)_serializer.BytesToMessage(typeof(Error), _errorMessageSerializer.Deserialize(rawErrorMessage.Body));
                tasks.Add(RepublishErrorAsync(error, _queueParameters, cancellationToken));
                _logger.LogDebug("RetryErrorsAsync {0} {1}", error.Queue, error.Message);

                await Task.WhenAll(tasks);
            }
        }

        public async Task RepublishErrorAsync(Error error, QueueParameters parameters, CancellationToken cancellationToken)
        {
            try
            {
                if (error.BasicProperties.Headers != null)
                {
                    object tempCount;
                    if (error.BasicProperties.Headers.TryGetValue(_retryCountHeaderKey, out tempCount))
                    {
                        error.BasicProperties.Headers[_retryCountHeaderKey] = (long)tempCount + 1;
                    }
                    else
                    {
                        error.BasicProperties.Headers.Add(_retryCountHeaderKey, 1);
                    }
                }
                else
                {
                    error.BasicProperties.Headers = new Dictionary<string, object>();
                    error.BasicProperties.Headers.Add(_retryCountHeaderKey, 1);
                }

                if ((error.BasicProperties.Headers[_retryCountHeaderKey] is long ? (long)error.BasicProperties.Headers[_retryCountHeaderKey] : 0) > _queueParameters.MaxRetry)
                {
                    //implement move to error_error
                    await PublishToDeadQueue(error, cancellationToken);
                    return;
                }
                await PublishToRetryMessage(error, cancellationToken);
                //model.BasicPublish("", error.Queue, true, properties, body);
                //model.WaitForConfirmsOrDie();
            }
            catch (OperationInterruptedException ex)
            {
                _logger.LogError("RepublishErrorAsync OperationInterruptedException The exchange, '{0}', described in the error message does not exist on '{1}', '{2}' {3}", error.Exchange, parameters.HostName, parameters.VHost, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("RepublishErrorAsync Exception '{0}' '{1}', '{2}' {3}", error.Exchange, parameters.HostName, parameters.VHost, ex);
            }
        }

        public async Task ExecuteRetryAsync(CancellationToken cancellationToken = default)
        {
            var errorMessages = GetAllErrorQueue();

            await RetryErrorsAsync(errorMessages, cancellationToken);
        }

        public async Task ExecuteRetryByQueueName(string queueName, CancellationToken cancellationToken = default)
        {
            var errorMessages = GetMessagesFromQueue(queueName);
            await RetryErrorsAsync(errorMessages, cancellationToken);
        }

        public async Task PublishToDeadQueue(Error error, CancellationToken cancellationToken = default)
        {
            var exchangeError = await _bus.Advanced.ExchangeDeclareAsync(GetDeadQueueName(error), conf =>
            {
                conf.AsDurable(false);
                conf.AsAutoDelete(false);
                conf.WithType(ExchangeType.Direct);
            }
            , cancellationToken);
            var queueError = await _bus.Advanced.QueueDeclareAsync(GetDeadQueueName(error), conf =>
            {
                conf.AsDurable(false);
                conf.AsAutoDelete(false);
            }, cancellationToken);
            var body = _errorMessageSerializer.Deserialize(error.Message);
            var binderErrror = await _bus.Advanced.BindAsync(exchangeError, queueError, "", cancellationToken);
            await _bus.Advanced.PublishAsync(exchangeError, "", true, error.BasicProperties, body, cancellationToken);
            _logger.LogDebug("ErrorRetry PublishToDeadQueue {0} {1}", error.Queue, error.Message);
        }

        public string GetDeadQueueName(Error error)
        {
            return string.Format("{0}{1}", error.Exchange, _deadQueue);
        }

        public async Task PublishToRetryMessage(Error error, CancellationToken cancellationToken = default)
        {
            var exchangeError = await _bus.Advanced.ExchangeDeclareAsync(error.Exchange, conf =>
            {
                conf.AsDurable(true);
                conf.AsAutoDelete(false);
                conf.WithType(ExchangeType.Topic);
            }
            , cancellationToken);
            var body = _errorMessageSerializer.Deserialize(error.Message);
            _logger.LogDebug("ErrorRetry PublishToRetryMessage {0} {1}", error.Queue, error.Message);
            await _bus.Advanced.PublishAsync(exchangeError, error.Queue, true, error.BasicProperties, body, cancellationToken);
        }
    }
}