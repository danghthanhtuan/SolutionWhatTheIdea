using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;
using SWTI.Products.Domain.Domain;
using SWTI.Products.Domain.Repositories;

namespace SWTI.Products.Domain
{
    public static class DependencyRegister
    {
        public static void ProductDomainDependencyRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGetProductDomain, GetProductDomain>();
            services.AddScoped<ICreateProductDomain, CreateProductDomain>();
            services.AddScoped<IUpdateProductDomain, UpdateProductDomain>();
            services.AddSingleton<IProductRepository, ProductRepository>();
        }
    }
}