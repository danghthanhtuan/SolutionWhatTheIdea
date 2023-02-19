using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using EasyNetQ.Consumer;
using EasyNetQ.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWTI.Interfaces.IQueue;
using SWTI.QueueCore;

namespace SWTI.Queue
{
    public static class DependencyRegister
    {
        public static void QueueDependencyRegister(this IServiceCollection services, IConfiguration Configuration)
        {
            RabbitMqConfigration rabbitMqConfigration = Configuration.GetSection("RabbitMqConfigration").Get<RabbitMqConfigration>();
            services.AddSingleton(rabbitMqConfigration);
            services.RegisterEasyNetQ((IServiceResolver connectionConfigurationFactory) => new ConnectionConfiguration
            {
                Hosts = new List<HostConfiguration>
                {
                    new HostConfiguration
                    {
                        Host = rabbitMqConfigration.Hosts[0].Host,
                        Port = rabbitMqConfigration.Hosts[0].Port
                    }
                },
                PrefetchCount = rabbitMqConfigration.PrefetchCount,
                UserName = rabbitMqConfigration.UserName,
                Password = rabbitMqConfigration.Password,
                VirtualHost = rabbitMqConfigration.VirtualHost,
                Timeout = TimeSpan.FromMilliseconds((int)rabbitMqConfigration.Timeout)
            }, delegate (IServiceRegister registerServices)
            {
                registerServices.EnableLegacyTypeNaming();
                registerServices.Register<IConsumerErrorStrategy, SConsumerErrorStrategy>();
            });
            services.AddSingleton((IConventions)new SConventions(new LegacyTypeNameSerializer())
            {
                ErrorExchangeNamingConvention = (MessageReceivedInfo info) => rabbitMqConfigration.ErrorQueueNaming,
                ErrorQueueNamingConvention = (MessageReceivedInfo info) => rabbitMqConfigration.ErrorQueueNaming
            });
            if (rabbitMqConfigration.IsConsume)
            {
                services.AutoSubscriberDependencyRegister(rabbitMqConfigration);
            }

            services.AddSingleton<IErrorRetry, ErrorRetry>();
            services.AddScoped<IQueueManagement, QueueManagement>();
            QueueParameters implementationInstance = new QueueParameters
            {
                HostName = rabbitMqConfigration.Hosts[0]?.Host,
                HostPort = rabbitMqConfigration.Hosts[0].Port,
                Password = rabbitMqConfigration.Password,
                Username = rabbitMqConfigration.UserName,
                QueueName = rabbitMqConfigration.ErrorQueueNaming,
                NumberOfMessagesToRetrieve = rabbitMqConfigration.MaxNumberOfMessageRetry,
                VHost = rabbitMqConfigration.VirtualHost,
                Purge = true,
                MaxRetry = rabbitMqConfigration.MaxRetry
            };
            services.AddSingleton(implementationInstance);
        }

        //
        // Summary:
        //     api có consume các queue hay không
        //
        // Parameters:
        //   services:
        //
        //   subscriptionIdPrefix:
        private static void AutoSubscriberDependencyRegister(this IServiceCollection services, RabbitMqConfigration rabbitMqConfigration)
        {
            services.AddSingleton<AutoSubscriberMessageDispatcher>();
            services.AddSingleton((IServiceProvider provider) => new AutoSubscriber(provider.GetRequiredService<IBus>(), rabbitMqConfigration.SubscriptionIdPrefix)
            {
                AutoSubscriberMessageDispatcher = provider.GetRequiredService<AutoSubscriberMessageDispatcher>(),
                ConfigureSubscriptionConfiguration = delegate (ISubscriptionConfiguration c)
                {
                    c.WithAutoDelete(autoDelete: false).WithPriority(10).WithPrefetchCount(rabbitMqConfigration.PrefetchCount);
                }
            });
        }

        public static void AutoRetryError(this IServiceCollection services, RabbitMqConfigration rabbitMqConfigration)
        {
        }

        public class AutoSubscriberMessageDispatcher : IAutoSubscriberMessageDispatcher
        {
            private readonly IServiceProvider serviceProvider;

            public AutoSubscriberMessageDispatcher(IServiceProvider serviceProvider)
            {
                this.serviceProvider = serviceProvider;
            }

            void IAutoSubscriberMessageDispatcher.Dispatch<TMessage, TConsumer>(TMessage message, CancellationToken cancellationToken)
            {
                using IServiceScope serviceScope = serviceProvider.CreateScope();
                TConsumer requiredService = serviceScope.ServiceProvider.GetRequiredService<TConsumer>();
                requiredService.Consume(message, cancellationToken);
            }

            async Task IAutoSubscriberMessageDispatcher.DispatchAsync<TMessage, TConsumer>(TMessage message, CancellationToken cancellationToken)
            {
                using IServiceScope scope = serviceProvider.CreateScope();
                TConsumer consumer = scope.ServiceProvider.GetRequiredService<TConsumer>();
                await consumer.ConsumeAsync(message, cancellationToken);
            }
        }
    }
}