using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWTI.Account.Domain;
using SWTI.Configurations;
using SWTI.Providers;

namespace SWTI.DependencyInjection
{
    public static class MainDependencyRegister
    {
        public static void AddMainDependencyRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigurationDependencyRegister(configuration);
            services.ProviderDependencyRegister(configuration);
            services.ServiceDependencyRegister(configuration);
            services.DomainDependencyRegister(configuration);       
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