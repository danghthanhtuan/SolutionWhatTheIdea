using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SWTI.ElasticSearch.Domain
{
    public static class DependencyRegister
    {
        public static void ElasticSearchDependencyRegister(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddScoped<ICreateAccountDomain, CreateAccountDomain>();
            //services.AddSingleton<IAccountRepository, AccountRepository>();
        }
    }
}