using EasyNetQ.Consumer;
using EasyNetQ;
using EasyNetQ.SystemMessages;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using SWTI.Interfaces.IQueue;
using SWTL.Models.Queues;
using Microsoft.Extensions.Logging;

namespace SWTI.QueueCore
{
    public class ErrorRetry : IErrorRetry
    {
        private readonly ILogger<ErrorRetry> _logger;

        private readonly IErrorMessageSerializer _errorMessageSerializer;

        private readonly QueueParameters _queueParameters;

        private readonly ISerializer _serializer;

        private readonly string _retryCountHeaderKey = "retry-count";

        private readonly IBus _bus;

        private readonly string _deadQueue = "_error_error";

        private readonly string _errorQueue = "_error";

        public ErrorRetry(ILogger<ErrorRetry> logger, IErrorMessageSerializer errorMessageSerializer, QueueParameters queueParameters, ISerializer serializer, IBus bus)
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
            foreach (string item in Storage.QueueNameStore)
            {
                list.AddRange(GetMessagesFromQueue(item + _errorQueue));
            }

            return list;
        }

        private IEnumerable<HosepipeMessage> GetMessagesFromQueue(string queueName)
        {
            using IConnection connection = HosepipeConnection.FromParameters(_queueParameters);
            using IModel channel = connection.CreateModel();
            _logger.LogDebug("GetMessagesFromQueue NumberOfMessagesToRetrieve: {0}", _queueParameters.NumberOfMessagesToRetrieve);
            channel.ConfirmSelect();
            int count = 0;
            while (count++ < _queueParameters.NumberOfMessagesToRetrieve)
            {
                BasicGetResult basicGetResult = channel.BasicGet(queueName, _queueParameters.Purge);
                if (basicGetResult == null)
                {
                    break;
                }

                HosepipeMessage hosepipeMessage = new HosepipeMessage(new MessageProperties(), info: new MessageReceivedInfo("hosepipe", basicGetResult.DeliveryTag, basicGetResult.Redelivered, basicGetResult.Exchange, basicGetResult.RoutingKey, queueName), body: _errorMessageSerializer.Serialize(basicGetResult.Body.ToArray()));
                _logger.LogDebug("GetMessagesFromQueue {0} {1} {2}", basicGetResult.Exchange, hosepipeMessage.Body, queueName);
                yield return hosepipeMessage;
            }
        }

        public async Task RetryErrorsAsync(IEnumerable<HosepipeMessage> rawErrorMessages, CancellationToken cancellationToken)
        {
            List<Task> tasks = new List<Task>();
            foreach (HosepipeMessage rawErrorMessage in rawErrorMessages)
            {
                Error error = (Error)_serializer.BytesToMessage(typeof(Error), _errorMessageSerializer.Deserialize(rawErrorMessage.Body));
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
                    if (error.BasicProperties.Headers.TryGetValue(_retryCountHeaderKey, out var tempCount))
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

                if (((error.BasicProperties.Headers[_retryCountHeaderKey] is long) ? ((long)error.BasicProperties.Headers[_retryCountHeaderKey]) : 0) <= _queueParameters.MaxRetry)
                {
                    await PublishToRetryMessage(error, cancellationToken);
                }
                else
                {
                    await PublishToDeadQueue(error, cancellationToken);
                }
            }
            catch (OperationInterruptedException ex3)
            {
                OperationInterruptedException ex2 = ex3;
                _logger.LogError("RepublishErrorAsync OperationInterruptedException The exchange, '{0}', described in the error message does not exist on '{1}', '{2}' {3}", error.Exchange, parameters.HostName, parameters.VHost, ex2);
            }
            catch (Exception ex4)
            {
                Exception ex = ex4;
                _logger.LogError("RepublishErrorAsync Exception '{0}' '{1}', '{2}' {3}", error.Exchange, parameters.HostName, parameters.VHost, ex);
            }
        }

        public async Task ExecuteRetryAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<HosepipeMessage> errorMessages = GetAllErrorQueue();
            await RetryErrorsAsync(errorMessages, cancellationToken);
        }

        public async Task ExecuteRetryByQueueName(string queueName, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<HosepipeMessage> errorMessages = GetMessagesFromQueue(queueName);
            await RetryErrorsAsync(errorMessages, cancellationToken);
        }

        public async Task PublishToDeadQueue(Error error, CancellationToken cancellationToken = default(CancellationToken))
        {
            EasyNetQ.Topology.Exchange exchangeError = await _bus.Advanced.ExchangeDeclareAsync(GetDeadQueueName(error), delegate (IExchangeDeclareConfiguration conf)
            {
                conf.AsDurable(isDurable: false);
                conf.AsAutoDelete(isAutoDelete: false);
                conf.WithType("direct");
            }, cancellationToken);
            EasyNetQ.Topology.Queue queueError = await _bus.Advanced.QueueDeclareAsync(GetDeadQueueName(error), delegate (IQueueDeclareConfiguration conf)
            {
                conf.AsDurable(isDurable: false);
                conf.AsAutoDelete(isAutoDelete: false);
            }, cancellationToken);
            byte[] body = _errorMessageSerializer.Deserialize(error.Message);
            await _bus.Advanced.BindAsync(exchangeError, queueError, "", cancellationToken);
            await _bus.Advanced.PublishAsync(exchangeError, "", mandatory: true, error.BasicProperties, body, cancellationToken);
            _logger.LogDebug("ErrorRetry PublishToDeadQueue {0} {1}", error.Queue, error.Message);
        }

        public string GetDeadQueueName(Error error)
        {
            return $"{error.Exchange}{_deadQueue}";
        }

        public async Task PublishToRetryMessage(Error error, CancellationToken cancellationToken = default(CancellationToken))
        {
            EasyNetQ.Topology.Exchange exchangeError = await _bus.Advanced.ExchangeDeclareAsync(error.Exchange, delegate (IExchangeDeclareConfiguration conf)
            {
                conf.AsDurable(isDurable: true);
                conf.AsAutoDelete(isAutoDelete: false);
                conf.WithType("topic");
            }, cancellationToken);
            byte[] body = _errorMessageSerializer.Deserialize(error.Message);
            _logger.LogDebug("ErrorRetry PublishToRetryMessage {0} {1}", error.Queue, error.Message);
            await _bus.Advanced.PublishAsync(exchangeError, error.Queue, mandatory: true, error.BasicProperties, body, cancellationToken);
        }
    }
}
