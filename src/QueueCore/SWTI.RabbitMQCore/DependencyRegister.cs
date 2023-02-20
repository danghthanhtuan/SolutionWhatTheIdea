using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using EasyNetQ.Consumer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWTI.EasyNetQ.Hosepipe;
using System;
using System.Collections.Generic;

namespace SWTI.RabbitMQCore
{
    public static class DependencyRegister
    {
        public static void QueueDependencyRegister(this IServiceCollection services, IConfiguration Configuration)
        {
            var rabbitMqConfigration = Configuration.GetSection(nameof(RabbitMqConfigration))
                .Get<RabbitMqConfigration>();

            services.AddSingleton(rabbitMqConfigration);

            services.RegisterEasyNetQ(connectionConfigurationFactory =>
            {
                return new ConnectionConfiguration
                {
                    Hosts = new List<HostConfiguration>() { new HostConfiguration { Host = rabbitMqConfigration.Hosts[0].Host, Port = rabbitMqConfigration.Hosts[0].Port } },
                    PrefetchCount = rabbitMqConfigration.PrefetchCount,
                    UserName = rabbitMqConfigration.UserName,
                    Password = rabbitMqConfigration.Password,
                    VirtualHost = rabbitMqConfigration.VirtualHost,
                    Timeout = TimeSpan.FromMilliseconds(rabbitMqConfigration.Timeout),
                }; ;
            }, registerServices =>
            {
                registerServices.EnableLegacyTypeNaming();
                registerServices.Register(typeof(IConsumerErrorStrategy), typeof(SConsumerErrorStrategy));
            });
            services.AddSingleton<IConventions>(new SConventions(new LegacyTypeNameSerializer())
            {
                ErrorExchangeNamingConvention = (info) => rabbitMqConfigration.ErrorQueueNaming,
                ErrorQueueNamingConvention = (info) => rabbitMqConfigration.ErrorQueueNaming,
            });
            if (rabbitMqConfigration.IsConsume)
            {
                services.AutoSubscriberDependencyRegister(rabbitMqConfigration);
                //services.AddHostedService<AutoSubscriberHostedService>(); // move qua program, chạy sau khi đã startup app xong
            }
            services.AddSingleton<IErrorRetry, ErrorRetry>();
            services.AddScoped<IQueueManagement, QueueManagement>();
            var _queueParameters = new QueueParameters
            {
                HostName = rabbitMqConfigration.Hosts[0]?.Host,
                HostPort = rabbitMqConfigration.Hosts[0].Port,
                Password = rabbitMqConfigration.Password,
                Username = rabbitMqConfigration.UserName,
                QueueName = rabbitMqConfigration.ErrorQueueNaming,
                NumberOfMessagesToRetrieve = rabbitMqConfigration.MaxNumberOfMessageRetry,
                VHost = rabbitMqConfigration.VirtualHost,
                Purge = true,
                MaxRetry = rabbitMqConfigration.MaxRetry,
            };
            services.AddSingleton(_queueParameters);
        }

        /// <summary>
        /// api có consume các queue hay không
        /// </summary>
        /// <param name="services"></param>
        /// <param name="subscriptionIdPrefix"></param>
        private static void AutoSubscriberDependencyRegister(this IServiceCollection services
            , RabbitMqConfigration rabbitMqConfigration)
        {
            services.AddSingleton<AutoSubscriberMessageDispatcher>();
            services.AddSingleton(provider =>
            {
                var subscriber = new AutoSubscriber(provider.GetRequiredService<IBus>(), rabbitMqConfigration.SubscriptionIdPrefix)
                {
                    AutoSubscriberMessageDispatcher = provider.GetRequiredService<AutoSubscriberMessageDispatcher>(),
                    ConfigureSubscriptionConfiguration = c => c.WithAutoDelete(false)
                                               .WithPriority(10)
                                               .WithPrefetchCount(rabbitMqConfigration.PrefetchCount),
                };
                //subscriber.SubscribeAsync(System.AppDomain.CurrentDomain.GetAssemblies());
                return subscriber;
            });
        }

        public static void AutoRetryError(this IServiceCollection services
            , RabbitMqConfigration rabbitMqConfigration)
        {
            //services.AddSingleton<IErrorMessageSerializer, DefaultErrorMessageSerializer>();
            //var queueParamters = new EasyNetQ.Hosepipe.QueueParameters
            //{
            //    HostName = rabbitMqConfigration.Hosts.FirstOrDefault().Host,
            //    HostPort = rabbitMqConfigration.Hosts.FirstOrDefault().Port,
            //    Password = rabbitMqConfigration.Password,
            //    Username = rabbitMqConfigration.UserName,
            //    QueueName = rabbitMqConfigrationExtend.ErrorQueueNaming,
            //    NumberOfMessagesToRetrieve = rabbitMqConfigrationExtend.MaxNumberOfMessageRetry,
            //    VHost = rabbitMqConfigrationExtend.VirtualHost
            //};
            //services.AddSingleton<EasyNetQ.Hosepipe.QueueParameters>(queueParamters);
            //services.AddSingleton<IErrorRetry, ErrorRetry>();
        }
    }
}
