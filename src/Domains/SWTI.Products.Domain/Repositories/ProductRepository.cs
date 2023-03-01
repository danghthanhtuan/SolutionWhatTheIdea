using Dapper;
using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IProviders;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.ModelDapper.Products;
using System.Data;

namespace SWTI.Products.Domain.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ILogger<ProductRepository> _logger;
        private readonly IProductIntroduceDBContext _dBContext;

        public ProductRepository(ILogger<ProductRepository> logger
            , IProductIntroduceDBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        public async Task<(int, BaseResponse?)> CreateProduct(CreateProductDapper request, CancellationToken cancellationToken)
        {
            try
            {
                var sqlCreate = @$"INSERT INTO {nameof(Products)}
                                           (ProductCode,
                                           ProductName,
                                           CategoryId,
                                           Description, 
                                           PartnerID,
                                           IsNew,
                                           IsHot,
                                           Content,
                                           Price,
                                           PromotionPrice,
                                           Video,
                                           CreatedDate
                                            )
                                   VALUES (@ProductCode,
                                           @ProductName,
                                           @CategoryId,
                                           @Description, 
                                           @PartnerID,
                                           @IsNew,
                                           @IsHot,
                                           @Content,
                                           @Price,
                                           @PromotionPrice,
                                           @Video,
                                           @CreatedDate
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
