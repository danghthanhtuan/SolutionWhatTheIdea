using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWTL.Configurations;

namespace SWTI.Configurations
{
    public static class DependencyRegister
    {
        public static void ConfigurationDependencyRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConnectionStrings>(configuration.GetSection(nameof(ConnectionStrings)));
        }
    }
}
