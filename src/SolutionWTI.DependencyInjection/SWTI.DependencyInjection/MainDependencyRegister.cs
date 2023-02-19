using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWTI.Account.Domain;
using SWTI.Configurations;
using SWTI.Consumers;
using SWTI.Mapper;
using SWTI.Providers;
using SWTI.RabbitMQCore;
using SWTI.RedisProvider;

namespace SWTI.DependencyInjection
{
    public static class MainDependencyRegister
    {
        public static void AddMainDependencyRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.QueueDependencyRegister(configuration);
            services.MapperDependencyRegister(configuration);
            services.ConfigurationDependencyRegister(configuration);
            services.ProviderDependencyRegister(configuration);
            services.ServiceDependencyRegister(configuration);
            services.DomainDependencyRegister(configuration);
            services.AddHostedService<SWTI.RabbitMQCore.AutoSubscriberHostedService>();
            services.ConsumerDependencyRegister();
            services.RedisDependencyRegister(configuration);
            
        }

        private static void ServiceDependencyRegister(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AccountSenpayClientDependencyRegister(configuration);
        }

        private static void DomainDependencyRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.AccountDomainDependencyRegister(configuration);
        }
    }
}