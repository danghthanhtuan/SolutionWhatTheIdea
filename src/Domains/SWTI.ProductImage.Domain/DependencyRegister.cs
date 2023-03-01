using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;
using SWTI.ProductImage.Domain.Domain;
using SWTI.ProductImage.Domain.Repositories;

namespace SWTI.ProductImage.Domain
{
    public static class DependencyRegister
    {
        public static void ProductImageDomainDependencyRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGetProductImageDomain, GetProductImageDomain>();
            services.AddScoped<ICreateProductImageDomain, CreateProductImageDomain>();
            services.AddScoped<IUpdateProductImageDomain, UpdateProductImageDomain>();
            services.AddSingleton<IProductImageRepository, ProductImageRepository>();
        }
    }
}