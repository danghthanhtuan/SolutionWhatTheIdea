using EasyNetQ.AutoSubscribe;
using EasyNetQ.Consumer;
using Microsoft.Extensions.Logging;
using SWTI.RabbitMQCore;
using SWTL.Models.ConsumerRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWTI.Consumers
{
    public class TestConsumer : IConsumeAsync<TestConsumerRequest>
    {
        private readonly ILogger<TestConsumer> _logger;

        public TestConsumer(ILogger<TestConsumer> logger
            )
        {
            _logger = logger;
        }

        public async Task ConsumeAsync(TestConsumerRequest message, CancellationToken cancellationToken = default)
        {
            await ConsumeHelper.ExecutionActionAsync(nameof(TestConsumer), _logger, async () =>
            {
                _logger.LogDebug("TestConsumer {0}", message.Dump());
                //var (result, err) = await _client.CallbackEKyc(message, cancellationToken);
                //_logger.LogInformation("TestConsumer {0} {1} {2}", result, err.Dump(), message.Dump());
                //err?.ThrowExceptionIfHasError();
            });
        }
    }
}
