using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IProviders;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.Requests.Account;

namespace SWTI.Account.Domain.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ILogger<AccountRepository> _logger;
        private readonly IProductIntroduceDBContext _dBContext;

        public AccountRepository(ILogger<AccountRepository> logger
            , IProductIntroduceDBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        public async Task<(int, BaseResponse)> CreateAccount(CreateAccountRequest req, CancellationToken cancellationToken)
        {
            try
            {
                //req.FromDate = new DateTime(req.FromDate.Year, req.FromDate.Month, req.FromDate.Day, 0, 0, 0);
                //using var connection = _dBRSContext.CreateConnection();
                //await connection.OpenAsync(cancellationToken);
                //var result = await connection.ExecuteScalarAsync<int>("usp_ProviderFee_Insert", req, commandType: CommandType.StoredProcedure);
                return (1, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AccountRepository InsertAccount {ex} {req.Dump()}");
                return (-1, BaseResponseExt.Error(500, ex.Message));
            }
        }
    }
}
