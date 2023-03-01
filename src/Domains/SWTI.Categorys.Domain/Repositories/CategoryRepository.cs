using Dapper;
using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IProviders;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.ModelDapper.Categories;
using SWTL.Models.Requests.Categories;
using System.Data;

namespace SWTI.Categories.Domain.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ILogger<CategoryRepository> _logger;
        private readonly IProductIntroduceDBContext _dBContext;

        public CategoryRepository(ILogger<CategoryRepository> logger
            , IProductIntroduceDBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        public async Task<(int, BaseResponse?)> CreateCategory(CreateCategoryDapper request, CancellationToken cancellationToken)
        {
            try
            {
                var sqlCreate = @$"INSERT INTO {nameof(SWTL.Models.Entities.Categories)}
                                           (
                                           CategoryName,
                                           CategoryParent,
                                           Status,
                                           SortOrder,
                                           CreatedDate)
                                   VALUES (@CategoryName,
                                           @CategoryParent,
                                           0,
                                           @SortOrder,
                                           GETDATE());";

                using var connection = _dBContext.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var result = await connection.ExecuteScalarAsync<int>(sqlCreate, request, commandType: CommandType.Text);
                return (result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(CategoryRepository)} CreateCategory {ex} {request.Dump()}");
                return (-1, BaseResponseExt.Error(500, ex.Message));
            }
        }

        public async Task<(SWTL.Models.Entities.Categories?, BaseResponse?)> GetCategoryByID(int id, CancellationToken cancellationToken)
        {
            try
            {
                var sqlSelect = @$"SELECT * 
                                   FROM {nameof(SWTL.Models.Entities.Categories)}
                                   WHERE ID = @ID;";

                using var connection = _dBContext.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var result = await connection.QueryFirstOrDefaultAsync<SWTL.Models.Entities.Categories>(sqlSelect, new { ID = id }, commandType: CommandType.Text);
                return (result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartnerRepository GetPartnerByCode {ex} ID ={id}");
                return (null, BaseResponseExt.Error(500, ex.Message));
            }
        }

        public async Task<(IEnumerable<SWTL.Models.Entities.Categories>?, int, BaseResponse?)> GetCategoryPaging(GetCategoryPagingRequest req, CancellationToken cancellationToken)
        {
            try
            {
                var where = " ";
                var query = $@"SELECT * FROM {nameof(SWTL.Models.Entities.Categories)}
                               WHERE 1 = 1 {where}
                               ORDER BY CreatedDate ASC 
                               OFFSET (@Page * @PageSize) - @PageSize ROWS
                               FETCH NEXT @PageSize ROWS ONLY; ";
                var queryCount = $"SELECT COUNT(ID) FROM {nameof(SWTL.Models.Entities.Categories)} WHERE 1 = 1 {where}";

                using var connection = _dBContext.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var result = await connection.QueryMultipleAsync(query + queryCount, req, commandType: CommandType.Text);

                var data = await result.ReadAsync<SWTL.Models.Entities.Categories>();
                var totalRows = await result.ReadFirstAsync<int>();

                return (data, totalRows, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"CategoryRepository GetCategoryPaging  {ex} {req.Dump()}");
                return (null, 0, BaseResponseExt.Error(500, ex.Message));
            }
        }

        public async Task<(int, BaseResponse?)> UpdateCategory(UpdateCategoryDapper request, CancellationToken cancellationToken)
        {
            try
            {
                var sqlCreate = @$"UPDATE {nameof(SWTL.Models.Entities.Categories)} " +
                                @" SET 
                                       CategoryName = @CategoryName,
                                       SortOrder = @SortOrder,
                                       Status = @Status,
                                       CategoryParent = @CategoryParent,
                                       UpdatedDate = GETDATE()
                                    WHERE ID = @CategoryId;";

                using var connection = _dBContext.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var result = await connection.ExecuteScalarAsync<int>(sqlCreate, request, commandType: CommandType.Text);
                return (result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartnerRepository UpdatePartner {ex} {request.Dump()}");
                return (-1, BaseResponseExt.Error(500, ex.Message));
            }
        }
    }
}
