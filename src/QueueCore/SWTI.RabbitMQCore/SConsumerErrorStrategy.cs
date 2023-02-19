using EasyNetQ.Consumer;
using EasyNetQ.Persistent;
using EasyNetQ.SystemMessages;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using EasyNetQ.Management.Client.Model;
using RabbitMQ.Client.Exceptions;
using EasyNetQ.Internals;
using System.Buffers;
using System.Collections.Concurrent;
using EasyNetQ;

namespace SWTI.RabbitMQCore
{
    public class SConsumerErrorStrategy : IConsumerErrorStrategy
    {
        private readonly ILogger<DefaultConsumerErrorStrategy> logger;
        private readonly IConsumerConnection connection;
        private readonly IConventions conventions;
        private readonly IErrorMessageSerializer errorMessageSerializer;
        private readonly ConcurrentDictionary<string, object> existingErrorExchangesWithQueues = new();
        private readonly ISerializer serializer;
        private readonly ITypeNameSerializer typeNameSerializer;
        private readonly ConnectionConfiguration configuration;
        private const string _errorName = Const.ErrorName;

        private volatile bool disposed;

        /// <summary>
        ///     Creates DefaultConsumerErrorStrategy
        /// </summary>
        public SConsumerErrorStrategy(
            ILogger<DefaultConsumerErrorStrategy> logger,
            IConsumerConnection connection,
            ISerializer serializer,
            IConventions conventions,
            ITypeNameSerializer typeNameSerializer,
            IErrorMessageSerializer errorMessageSerializer,
            ConnectionConfiguration configuration
        )
        {


            this.logger = logger;
            this.connection = connection;
            this.serializer = serializer;
            this.conventions = conventions;
            this.typeNameSerializer = typeNameSerializer;
            this.errorMessageSerializer = errorMessageSerializer;
            this.configuration = configuration;
        }

        /// <inheritdoc />
        public virtual Task<AckStrategy> HandleConsumerErrorAsync(ConsumerExecutionContext context, Exception exception, CancellationToken cancellationToken)
        {


            if (disposed)
                throw new ObjectDisposedException(nameof(DefaultConsumerErrorStrategy));

            var receivedInfo = context.ReceivedInfo;
            var properties = context.Properties;
            var body = context.Body.ToArray();

            logger.LogError(
                exception,
                "Exception thrown by subscription callback, receivedInfo={receivedInfo}, properties={properties}, message={message}",
                receivedInfo,
                properties,
                Convert.ToBase64String(body)
            );

            try
            {
                using var model = connection.CreateModel();
                if (configuration.PublisherConfirms) model.ConfirmSelect();

                var errorExchange = DeclareErrorExchangeWithQueue(model, receivedInfo);

                using var message = CreateErrorMessage(receivedInfo, properties, body, exception);

                var errorProperties = model.CreateBasicProperties();
                errorProperties.Persistent = true;
                errorProperties.Type = typeNameSerializer.Serialize(typeof(Error));
                logger.LogDebug("HandleConsumerErrorAsync start {0} {1} {2}", receivedInfo, properties, Convert.ToBase64String(body));
                model.BasicPublish(errorExchange, receivedInfo.RoutingKey, errorProperties, message.Memory);
                logger.LogInformation("HandleConsumerErrorAsync done {0} {1} {2}", receivedInfo, properties, Convert.ToBase64String(body));

                if (!configuration.PublisherConfirms) return Task.FromResult(AckStrategies.Ack);

                return Task.FromResult(model.WaitForConfirms(configuration.Timeout) ? AckStrategies.Ack : AckStrategies.NackWithRequeue);
            }
            catch (BrokerUnreachableException unreachableException)
            {
                // thrown if the broker is unreachable during initial creation.
                logger.LogError(
                    unreachableException,
                    "Cannot connect to broker while attempting to publish error message {0} {1}", unreachableException, context.Body.ToArray()
                );
            }
            catch (OperationInterruptedException interruptedException)
            {
                // thrown if the broker connection is broken during declare or publish.
                logger.LogError(
                    interruptedException,
                    "Broker connection was closed while attempting to publish error message {0} {1}", interruptedException, context.Body.ToArray()
                );
            }
            catch (Exception unexpectedException)
            {
                // Something else unexpected has gone wrong :(
                logger.LogError(unexpectedException, "Failed to publish error message {0} {1}", unexpectedException, context.Body.ToArray());
            }

            return Task.FromResult(AckStrategies.NackWithRequeue);
        }

        /// <inheritdoc />
        public virtual Task<AckStrategy> HandleConsumerCancelledAsync(ConsumerExecutionContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(AckStrategies.NackWithRequeue);
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            if (disposed) return;
            disposed = true;
        }

        private static void DeclareAndBindErrorExchangeWithErrorQueue(IModel model, string exchangeName, string queueName, string routingKey)
        {
            model.QueueDeclare(queueName, true, false, false, null);
            model.ExchangeDeclare(exchangeName, RabbitMQ.Client.ExchangeType.Direct, true);
            model.QueueBind(queueName, exchangeName, routingKey);
        }

        private string DeclareErrorExchangeWithQueue(IModel model, MessageReceivedInfo receivedInfo)
        {
            var errorExchangeName = receivedInfo.Exchange + _errorName;
            var errorQueueName = receivedInfo.Exchange + _errorName;
            var routingKey = receivedInfo.RoutingKey;

            var errorTopologyIdentifier = $"{errorExchangeName}-{errorQueueName}-{routingKey}";

            existingErrorExchangesWithQueues.GetOrAdd(errorTopologyIdentifier, _ =>
            {
                DeclareAndBindErrorExchangeWithErrorQueue(model, errorExchangeName, errorQueueName, routingKey);
                return null;
            });

            return errorExchangeName;
        }

        private IMemoryOwner<byte> CreateErrorMessage(
            MessageReceivedInfo receivedInfo, MessageProperties properties, byte[] body, Exception exception
        )
        {
            var messageAsString = errorMessageSerializer.Serialize(body);
            var error = new Error
            {
                RoutingKey = receivedInfo.RoutingKey,
                Exchange = receivedInfo.Exchange,
                Queue = receivedInfo.Queue,
                Exception = exception.ToString(),
                Message = messageAsString,
                DateTime = DateTime.UtcNow
            };

            if (properties.Headers == null)
            {
                error.BasicProperties = properties;
            }
            else
            {
                // we'll need to clone context.Properties as we are mutating the headers dictionary
                error.BasicProperties = (MessageProperties)properties.Clone();

                // the RabbitMQClient implicitly converts strings to byte[] on sending, but reads them back as byte[]
                // we're making the assumption here that any byte[] values in the headers are strings
                // and all others are basic types. RabbitMq client generally throws a nasty exception if you try
                // to store anything other than basic types in headers anyway.

                //see http://hg.rabbitmq.com/rabbitmq-dotnet-client/file/tip/projects/client/RabbitMQ.Client/src/client/impl/WireFormatting.cs

                error.BasicProperties.Headers = properties.Headers.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value is byte[] bytes ? Encoding.UTF8.GetString(bytes) : kvp.Value
                );
            }
            return serializer.MessageToBytes(typeof(Error), error);
        }
    }

}