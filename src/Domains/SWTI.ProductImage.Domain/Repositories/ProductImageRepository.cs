using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IProviders;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTI.Models.ModelDapper.ProductImages;
using System.Data;

namespace SWTI.ProductImage.Domain.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly ILogger<ProductImageRepository> _logger;
        private readonly IProductIntroduceDBContext _dBContext;

        public ProductImageRepository(ILogger<ProductImageRepository> logger
            , IProductIntroduceDBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        public async Task<(int, BaseResponse?)> CreatePartner(List<CreateProductImageDapper> reqs, CancellationToken cancellationToken)
        {
            try
            {
                var sqlCreate = @$"INSERT INTO {nameof(SWTI.Models.ModelDapper.ProductImages)}
                                           (Code,
                                           Name,
                                           UrlLogo,
                                           Description, 
                                           CreatedUser,
                                           Status,
                                           CreatedDate)
                                   VALUES (@Code,
                                           @Name,
                                           @UrlLogo,
                                           @Description, 
                                           @CreatedUser,
                                           0,
                                           GETDATE());";

                using var connection = _dBContext.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var result = await connection.ExecuteScalarAsync<int>(sqlCreate, request, commandType: CommandType.Text);
                return (result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartnerRepository CreatePartner {ex} {request.Dump()}");
                return (-1, BaseResponseExt.Error(500, ex.Message));
            }
        }

    }
}
