using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using EasyNetQ.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SWTI.RabbitMQCore
{
    /// <summary>
    /// AutoSubscriberHostedService
    /// </summary>
    public class AutoSubscriberHostedService : IHostedService
    {
        private readonly Microsoft.Extensions.Logging.ILogger<AutoSubscriberHostedService> _logger;
        private readonly AutoSubscriber _autoSubscriber;
        private readonly IBus _bus;

        public AutoSubscriberHostedService(
            Microsoft.Extensions.Logging.ILogger<AutoSubscriberHostedService> logger
            , AutoSubscriber autoSubscriber
            , IBus bus
       )
        {
            _logger = logger;
            _autoSubscriber = autoSubscriber;
            _bus = bus;
        }

        /// <summary>
        /// cho try catch để có thể start dc dù rabbitmq koh alive
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("AutoSubscriberHostedService StartAsync {0}", DateTime.Now);
            try
            {
                await _autoSubscriber.SubscribeAsync(AppDomain.CurrentDomain.GetAssemblies(), new CancellationToken());
            }
            catch (Exception ex)
            {
                _logger.LogError("AutoSubscriberHostedService StartAsync {0}", ex);
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("AutoSubscriberHostedService StopAsync");
            _bus.Dispose();
            return Task.CompletedTask;
        }
    }
}