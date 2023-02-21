using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWTI.Interfaces.IDomains;
using SWTI.UploadFileServer.Domain.Domain;

namespace SWTI.UploadFileServer.Domain
{
    public static class DependencyRegister
    {
        public static void UploadFileServeDomainDependencyRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUploadFileToServerDomain, UploadFileToServerDomain>();
        }
    }
}