using SWTI.Utils;
using SWTL.Models.Requests.Account;

namespace SWTI.Interfaces.IDomains
{
    public interface ICreateAccountDomain
    {
        Task<(int, BaseResponse)> CreateAccount(CreateAccountRequest req, CancellationToken cancellationToken);
    }
}
