using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWTI.Account.Domain.Domain;
using SWTI.Account.Domain.Repositories;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;

namespace SWTI.Account.Domain
{
    public static class DependencyRegister
    {
        public static void AccountDomainDependencyRegister(this IServiceCollection services, IConfiguration configuration)
        {   
            services.AddScoped<ICreateAccountDomain, CreateAccountDomain>();
            services.AddSingleton<IAccountRepository, AccountRepository>();
        }
    }
}