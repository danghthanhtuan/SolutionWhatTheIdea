using Dapper;
using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IProviders;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
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

        public async Task<(int, BaseResponse?)> CreatePartner(CreatePartnerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var sqlCreate = @$"INSERT INTO {nameof(SWTL.Models.Entities.Partner)}
                                   SET Code = @Code,
                                       Name = @Name,
                                       UrlLogo =@UrlLogo,
                                       Description = @Description, 
                                       CreatedUser = @CreatedUser
                                       Status = 0;";

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