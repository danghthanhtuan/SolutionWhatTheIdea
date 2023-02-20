using EasyNetQ;
using EasyNetQ.Consumer;
using EasyNetQ.Persistent;
using EasyNetQ.SystemMessages;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Buffers;
using System.Text;

namespace SWTI.QueueCore
{
    //
    // Summary:
    //     Dead-letter queues do not implement by the EasyNetQ library. The handling mechanism
    //     of the RabbitMQ failed messages is implemented in the HandleConsumerError class
    //     Here this functionality is overridden. From now on, all the failed messages instead
    //     of being moved to the default error queue will be moved to a newly and automatically
    //     created dead letter exchange and dead letter queue. Each Queue can possess its
    //     own specific dead letter queue, to achieve this, the Queue object should be decorated
    //     with the EasyDeadLetter attribute.
    public class SConsumerErrorStrategy : DefaultConsumerErrorStrategy
    {
        private const bool _durableQueue = true;

        private const bool _autoDelete = false;

        private const string _routingKey = "#";

        private readonly IPersistentConnection persistentConnection;

        private readonly ITypeNameSerializer _typeNameSerializer;

        private const string _errorName = "_error";

        private readonly Microsoft.Extensions.Logging.ILogger<SConsumerErrorStrategy> _logger;

        private readonly EasyNetQ.Logging.ILogger<DefaultConsumerErrorStrategy> _logger2;

        private readonly IErrorMessageSerializer _errorMessageSerializer;

        private readonly ISerializer _serializer;

        public SConsumerErrorStrategy(EasyNetQ.Logging.ILogger<DefaultConsumerErrorStrategy> logger2,
            IPersistentConnection persistentConnection
            , ISerializer serializer
            , IConventions conventions
            , ITypeNameSerializer typeNameSerializer
            , IErrorMessageSerializer errorMessageSerializer
            , Microsoft.Extensions.Logging.ILogger<SConsumerErrorStrategy> logger
            , ConnectionConfiguration configuration)
            : base(logger2, (IConsumerConnection)persistentConnection, serializer, conventions, typeNameSerializer, errorMessageSerializer, configuration)
        {
            this.persistentConnection = persistentConnection;
            _typeNameSerializer = typeNameSerializer;
            _logger = logger;
            _logger2 = logger2;
            _errorMessageSerializer = errorMessageSerializer;
            _serializer = serializer;
        }

        //
        // Summary:
        //     overriding the error handling mechanism by the new approach
        //
        // Parameters:
        //   context:
        //     includes message boody,ReceivedInformation
        //
        //   exception:
        //     The exception detail of the failure message consumer
        public async Task<AckStrategy> HandleConsumerError(ConsumerExecutionContext context, Exception exception, CancellationToken cancellationToken)
        {
            try
            {
                if (!context.ReceivedInfo.Redelivered)
                {
                    return AckStrategies.NackWithRequeue;
                }

                if (context.Properties.Type == null)
                {
                    return await base.HandleConsumerErrorAsync(context, exception, cancellationToken);
                }

                _logger.LogError("HandleConsumerError {0} {1} {2}", exception.Message, context.ReceivedInfo, exception.StackTrace, Encoding.UTF8.GetString(context.Body.ToArray()));
                using IModel model = persistentConnection.CreateModel();
                string text = DeclareDeadLetterExchangeAndQueue(model, context.ReceivedInfo);
                IBasicProperties basicProperties = model.CreateBasicProperties();
                basicProperties.Persistent = true;
                basicProperties.Type = _typeNameSerializer.Serialize(typeof(Error));
                _logger.LogDebug("HandleConsumerError BasicPublish {0} {1} {2} {3}", text, context.ReceivedInfo.RoutingKey, basicProperties, Encoding.UTF8.GetString(context.Body.ToArray()));
                byte[] array = CreateErrorMessage(context.ReceivedInfo, context.Properties, context.Body, exception);
                model.BasicPublish(text, context.ReceivedInfo.RoutingKey, basicProperties, array);
                return AckStrategies.Ack;
            }
            catch (Exception ex)
            {
                AggregateException exception2 = new AggregateException("EasyDeadLetter exception:" + ex?.Message + ".Original Exception :" + exception.Message, ex?.InnerException, exception?.InnerException);
                return await base.HandleConsumerErrorAsync(context, exception2, cancellationToken);
            }
        }

        //
        // Summary:
        //     Create a new DeadLetter Exhcange and queue ,in case it does not exist
        //
        // Parameters:
        //   model:
        //     RabbitMQ model instance
        //
        //   deadLetterInfo:
        //     deadLetter attribute detail
        //
        // Exceptions:
        //   T:System.Exception:
        //     In case of null value for te DeadLetter attribute , the exception will be thrown
        //     and the message will be processed by the default mechanism of EasyNetQ
        private string DeclareDeadLetterExchangeAndQueue(IModel model, MessageReceivedInfo receivedInfo)
        {
            string text = receivedInfo.Exchange + "_error";
            string exchange = receivedInfo.Exchange + "_error";
            model.ExchangeDeclare(exchange, "topic", durable: true, autoDelete: false, null);
            model.QueueDeclare(text, durable: true, exclusive: false, autoDelete: false, null);
            model.QueueBind(text, exchange, "#");
            return text;
        }

        private byte[] CreateErrorMessage(MessageReceivedInfo receivedInfo, MessageProperties properties, ReadOnlyMemory<byte> body, Exception exception)
        {
            string message = _errorMessageSerializer.Serialize(body.ToArray());
            Error error = new Error
            {
                RoutingKey = receivedInfo.RoutingKey,
                Exchange = receivedInfo.Exchange,
                Queue = receivedInfo.Queue,
                Exception = exception.ToString(),
                Message = message,
                DateTime = DateTime.UtcNow
            };
            if (properties.Headers == null)
            {
                error.BasicProperties = properties;
            }
            else
            {
                error.BasicProperties = (MessageProperties)properties.Clone();
                error.BasicProperties.Headers = properties.Headers.ToDictionary((KeyValuePair<string, object> kvp) => kvp.Key, delegate (KeyValuePair<string, object> kvp)
                {
                    byte[] array = kvp.Value as byte[];
                    return (array != null) ? Encoding.UTF8.GetString(array) : kvp.Value;
                });
            }

            return _serializer.MessageToBytes(typeof(Error), error).Memory.ToArray();
        }
    }
}
