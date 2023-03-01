using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWTI.Categories.Domain.Domain;
using SWTI.Categories.Domain.Repositories;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;

namespace SWTI.Categories.Domain
{
    public static class DependencyRegister
    {
        public static void CategoryDomainDependencyRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGetCategoryDomain, GetCategoryDomain>();
            services.AddScoped<ICreateCategoryDomain, CreateCategoryDomain>();
            services.AddScoped<IUpdateCategoryDomain, UpdateCategoryDomain>();
            services.AddSingleton<ICategoryRepository, CategoryRepository>();
        }
    }
}