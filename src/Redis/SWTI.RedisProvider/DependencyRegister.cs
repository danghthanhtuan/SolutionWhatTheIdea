using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWTI.Configurations;

namespace SWTI.RedisProvider
{
    public static class DependencyRegister
    {
        public static void RedisDependencyRegister(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection(nameof(RedisSettings)).Get<RedisSettings>();
            services.AddSingleton<SWTIRedis>(new SWTIRedis(config.RedisServerAddress, config.IsTwemproxy, config.RedisTimeOut));
            //services.AddScoped<IRedisClient, RedisClient>();
        }
    }
}
