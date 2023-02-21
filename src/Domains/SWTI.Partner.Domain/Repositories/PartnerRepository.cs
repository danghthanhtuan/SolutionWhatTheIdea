using Dapper;
using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IProviders;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.ModelDapper;
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
                var sqlCreate = @$"INSERT INTO {nameof(SWTL.Models.Entities.Partner)}
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

        public async Task<(SWTL.Models.Entities.Partner?, BaseResponse?)> GetPartnerByCode(string code, CancellationToken cancellationToken)
        {
            try
            {
                var sqlSelect = @$"SELECT * 
                                   FROM {nameof(SWTL.Models.Entities.Partner)}
                                   WHERE Code = @Code;";

                using var connection = _dBContext.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var result = await connection.QueryFirstOrDefaultAsync<SWTL.Models.Entities.Partner>(sqlSelect, new { Code = code }, commandType: CommandType.Text);
                return (result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartnerRepository GetPartnerByCode {ex} code {code}");
                return (null, BaseResponseExt.Error(500, ex.Message));
            }
        }

        public async Task<(SWTL.Models.Entities.Partner?, BaseResponse?)> GetPartnerByID(int id, CancellationToken cancellationToken)
        {
            try
            {
                var sqlSelect = @$"SELECT * 
                                   FROM {nameof(SWTL.Models.Entities.Partner)}
                                   WHERE ID = @Code;";

                using var connection = _dBContext.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var result = await connection.QueryFirstOrDefaultAsync<SWTL.Models.Entities.Partner>(sqlSelect, new { ID = id }, commandType: CommandType.Text);
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
                var sqlCreate = @$"UPDATE{nameof(SWTL.Models.Entities.Partner)} " +
                                @" SET Code = @Code,
                                       Name = @Name,
                                       UrlLogo = @UrlLogo,
                                       Description = @Description, 
                                       UpdatedUser = @UpdatedUser,
                                       Status = @Status,
                                       UpdatedDate = GETDATE()
                                    WHERE ID = @ID;";

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