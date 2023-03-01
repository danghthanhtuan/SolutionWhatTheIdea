using Dapper;
using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IProviders;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.Entities;
using SWTL.Models.ModelDapper.Products;
using SWTL.Models.Requests.Partner;
using SWTL.Models.Requests.Products;
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

        public async Task<(SWTL.Models.Entities.Products?, BaseResponse?)> GetProductByID(int id, CancellationToken cancellationToken)
        {
            try
            {
                var sqlSelect = @$"SELECT * 
                                   FROM {nameof(SWTL.Models.Entities.Products)}
                                   WHERE ID = @ID;";

                using var connection = _dBContext.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var result = await connection.QueryFirstOrDefaultAsync<SWTL.Models.Entities.Products>(sqlSelect, new { ID = id }, commandType: CommandType.Text);
                return (result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartnerRepository GetPartnerByCode {ex} ID ={id}");
                return (null, BaseResponseExt.Error(500, ex.Message));
            }
        }
        public async Task<(IEnumerable<SWTL.Models.Entities.Products>, int, BaseResponse)> GetProductPaging(GetProductPagingRequest req, CancellationToken cancellationToken)
        {
            try
            {
                var where = " ";
                if (req.DateFrom.HasValue)
                {
                    where += " AND CreatedDate >= @DateFrom ";
                }
                if (req.DateTo.HasValue)
                {
                    where += " AND CreatedDate <= @DateTo ";
                }
                if (string.IsNullOrEmpty(req.ProductName) == false)
                {
                    where += $" AND ProductName = @ProductName ";
                }
                if (req.CategoryId > 0)
                {
                    where += $" AND CategoryId = @CategoryId ";
                }
               
                var query = $@"SELECT * FROM {nameof(SWTL.Models.Entities.Products)}
                               WHERE 1 = 1 {where}
                               ORDER BY CreatedDate ASC 
                               OFFSET (@Page * @PageSize) - @PageSize ROWS
                               FETCH NEXT @PageSize ROWS ONLY; ";
                var queryCount = $"SELECT COUNT(ID) FROM {nameof(SWTL.Models.Entities.Products)} WHERE 1 = 1 {where}";

                using var connection = _dBContext.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var result = await connection.QueryMultipleAsync(query + queryCount, req, commandType: CommandType.Text);

                var data = await result.ReadAsync<SWTL.Models.Entities.Products>();
                var totalRows = await result.ReadFirstAsync<int>();

                _logger.LogDebug($"{nameof(ProductRepository)} {result.Dump()} => {req.Dump()} {query}");
                return (data, totalRows, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ProductRepository)}  {ex} {req.Dump()}");
                return (null, 0, BaseResponseExt.Error(500, ex.Message));
            }
        }

    }
}
