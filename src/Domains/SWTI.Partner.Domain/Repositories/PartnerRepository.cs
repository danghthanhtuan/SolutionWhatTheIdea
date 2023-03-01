using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IProviders;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.Entities;
using SWTL.Models.ModelDapper;
using SWTL.Models.Requests.Partner;
using System.Data;

namespace SWTI.Partner.Domain.Repositories
{
    public class PartnerRepository : IPartnerRepository
    {
        private readonly ILogger<PartnerRepository> _logger;
        private readonly IProductIntroduceDBContext _dBContext;

        public PartnerRepository(ILogger<PartnerRepository> logger
            , IProductIntroduceDBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        public async Task<(int, BaseResponse?)> CreatePartner(CreatePartnerDapper request, CancellationToken cancellationToken)
        {
            try
            {
                var sqlCreate = @$"INSERT INTO {nameof(SWTL.Models.Entities.Partners)}
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

        public async Task<(IEnumerable<SWTL.Models.Entities.Partners>, int,  BaseResponse)> GetPartnePaging(GetPartnerPagingRequest req, CancellationToken cancellationToken)
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
                if (string.IsNullOrEmpty(req.Name) == false)
                {
                    where += $" AND Name = @Name ";
                }
                if (string.IsNullOrEmpty(req.Phone) == false)
                {
                    where += $" AND PhoneNumber = @Phone ";
                }
                if (string.IsNullOrEmpty(req.Code) == false)
                {
                    where += $" AND Code = @Code ";
                }
                var query = $@"SELECT * FROM {nameof(Partners)}
                               WHERE 1 = 1 {where}
                               ORDER BY CreatedDate ASC 
                               OFFSET (@Page * @PageSize) - @PageSize ROWS
                               FETCH NEXT @PageSize ROWS ONLY; ";
                var queryCount = $"SELECT COUNT(ID) FROM {nameof(Partners)} WHERE 1 = 1 {where}";

                using var connection = _dBContext.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var result = await connection.QueryMultipleAsync(query + queryCount, req, commandType: CommandType.Text);

                var data = await result.ReadAsync<Partners>();
                var totalRows = await result.ReadFirstAsync<int>();

                _logger.LogDebug($"PartnerRepository {result.Dump()} => {req.Dump()} {query}");
                return (data, totalRows, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartnerRepository  {ex} {req.Dump()}");
                return (null, 0, BaseResponseExt.Error(500, ex.Message));
            }
        }

        public async Task<(SWTL.Models.Entities.Partners?, BaseResponse?)> GetPartnerByCode(string code, CancellationToken cancellationToken)
        {
            try
            {
                var sqlSelect = @$"SELECT * 
                                   FROM {nameof(SWTL.Models.Entities.Partners)}
                                   WHERE Code = @Code;";

                using var connection = _dBContext.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var result = await connection.QueryFirstOrDefaultAsync<SWTL.Models.Entities.Partners>(sqlSelect, new { Code = code }, commandType: CommandType.Text);
                return (result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartnerRepository GetPartnerByCode {ex} code {code}");
                return (null, BaseResponseExt.Error(500, ex.Message));
            }
        }

        public async Task<(SWTL.Models.Entities.Partners?, BaseResponse?)> GetPartnerByID(int id, CancellationToken cancellationToken)
        {
            try
            {
                var sqlSelect = @$"SELECT * 
                                   FROM {nameof(SWTL.Models.Entities.Partners)}
                                   WHERE ID = @ID;";

                using var connection = _dBContext.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var result = await connection.QueryFirstOrDefaultAsync<SWTL.Models.Entities.Partners>(sqlSelect, new { ID = id }, commandType: CommandType.Text);
                return (result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartnerRepository GetPartnerByCode {ex} ID ={id}");
                return (null, BaseResponseExt.Error(500, ex.Message));
            }
        }

        public async Task<(int, BaseResponse)> UpdatePartner(UpdatePartnerDapper request, CancellationToken cancellationToken)
        {
            try
            {
                var sqlCreate = @$"UPDATE {nameof(SWTL.Models.Entities.Partners)} " +
                                @" SET 
                                       Name = @Name,
                                       UrlLogo = @UrlLogo,
                                       Description = @Description, 
                                       UpdatedUser = @UpdatedUser,
                                       Status = @Status,
                                       UpdatedDate = GETDATE()
                                    WHERE ID = @PartnerId;";

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