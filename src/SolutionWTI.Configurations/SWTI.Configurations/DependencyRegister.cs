using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SWTI.Configurations
{
    public static class DependencyRegister
    {
        public static void ConfigurationDependencyRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConnectionStrings>(configuration.GetSection(nameof(ConnectionStrings)));
            services.Configure<RedisSettings>(configuration.GetSection(nameof(RedisSettings)));
            services.Configure<FolderImageConfig>(configuration.GetSection(nameof(FolderImageConfig)));
            services.Configure<MongoConnectionStrings>(configuration.GetSection(nameof(MongoConnectionStrings)));
            
        }
    }
}
