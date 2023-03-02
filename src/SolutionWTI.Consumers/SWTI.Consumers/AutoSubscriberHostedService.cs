using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using EasyNetQ.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SWTI.Consumers
{
    public class AutoSubscriberHostedService : BackgroundService
    {
        private readonly Microsoft.Extensions.Logging.ILogger<AutoSubscriberHostedService> _logger;

         private readonly AutoSubscriber _autoSubscriber;

        // private readonly IBus _bus;
        private readonly IServiceProvider _services;

        public AutoSubscriberHostedService(Microsoft.Extensions.Logging.ILogger<AutoSubscriberHostedService> logger, IServiceProvider services)// AutoSubscriber autoSubscriber, IBus bus)
        {
            _logger = logger;
            _services = services;
            //EasyNetQ.Logging.LogProvider.SetCurrentLogProvider(ConsoleLogProvider.Instance);
        }

        //
        // Summary:
        //     cho try catch để có thể start dc dù rabbitmq koh alive Triggered when the application
        //     host is ready to start the service.
        //
        // Parameters:
        //   cancellationToken:
        //     Indicates that the start process has been aborted.
        public async override Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(3000);

            _logger.LogInformation("AutoSubscriberHostedService StartAsync {0}", DateTime.Now);
            try
            {
                await _services.GetRequiredService<AutoSubscriber>().SubscribeAsync(AppDomain.CurrentDomain.GetAssemblies());
            }
            catch (Exception ex2)
            {
                Exception ex = ex2;
                _logger.LogError("AutoSubscriberHostedService StartAsync {0}", ex);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("AutoSubscriberHostedService StopAsync");
            _services.GetRequiredService<IBus>().Dispose();
            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return StartAsync(cancellationToken: stoppingToken);
        }
    }
}
