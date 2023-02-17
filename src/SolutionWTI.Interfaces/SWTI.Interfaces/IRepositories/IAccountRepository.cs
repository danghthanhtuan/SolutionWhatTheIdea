using SWTI.Utils;
using SWTL.Models.Requests.Account;

namespace SWTI.Interfaces.IRepositories
{
    public interface IAccountRepository
    {
        public Task<(int, BaseResponse)> CreateAccount(CreateAccountRequest req, CancellationToken cancellationToken);
    }
}
