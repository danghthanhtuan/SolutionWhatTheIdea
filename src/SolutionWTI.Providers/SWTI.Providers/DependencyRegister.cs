using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWTI.Interfaces.IProviders;

namespace SWTI.Providers
{
    public static class DependencyRegister
    {
        public static void ProviderDependencyRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IProductIntroduceDBContext, ProductIntroduceDBContext>();
            services.AddSingleton<IMongoDBContext, MongoDBContext>();
            
        }
    }
}
