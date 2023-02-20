using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SWTI.Mapper
{
    public static class DependencyRegister
    {
        public static void MapperDependencyRegister(this IServiceCollection services, IConfiguration configuration)
        {
            var config = new TypeAdapterConfig();

            //config.NewConfig<MBBankTxtFileResponse, BulkStatementRequest>()
            //    .Map(dest => dest.PartnerID, src => src.FT)
            //    .Map(dest => dest.PaymentID, src => src.RequestID)
            //    .Map(dest => dest.TransactionDate, src => src.BankTransactionDate)
            //    .Map(dest => dest.Amount, src => src.Amount);

            services.AddSingleton(config);
            services.AddSingleton<IMapper, ServiceMapper>();
        }
    }
}