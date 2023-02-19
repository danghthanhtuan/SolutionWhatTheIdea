using Microsoft.Extensions.DependencyInjection;

namespace SWTI.Consumers
{
    public static class DependencyRegister
    {
        public static void ConsumerDependencyRegister(this IServiceCollection services)
        {
            services.AddScoped<TestConsumer>();
        }
    }
}