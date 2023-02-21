using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;
using SWTI.Partner.Domain.Domain;
using SWTI.Partner.Domain.Repositories;

namespace SWTI.Partner.Domain
{
    public static class DependencyRegister
    {
        public static void PartnerDomainDependencyRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGetPartnerDomain, GetPartnerDomain>();
            services.AddScoped<ICreatePartnerDomain, CreatePartnerDomain>();
            services.AddSingleton<IPartnerRepository, PartnerRepository>();
        }
    }
}