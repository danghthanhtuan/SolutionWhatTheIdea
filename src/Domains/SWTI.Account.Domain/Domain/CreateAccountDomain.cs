using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.Requests.Account;

namespace SWTI.Account.Domain.Domain
{
    public class CreateAccountDomain : ICreateAccountDomain
    {
        private readonly ILogger<CreateAccountDomain> _logger;

        private readonly IAccountRepository _accountRepository;
        public CreateAccountDomain(
            ILogger<CreateAccountDomain> logger,
            IAccountRepository accountRepository)
        {
            _logger = logger;
            _accountRepository = accountRepository;
        }

        public async Task<(int, BaseResponse)> CreateAccount(CreateAccountRequest req, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"CreateAccountDomain CreateAccount req {req.Dump()}");
            await _accountRepository.CreateAccount(req, cancellationToken);
            throw new NotImplementedException();
        }
    }
}
