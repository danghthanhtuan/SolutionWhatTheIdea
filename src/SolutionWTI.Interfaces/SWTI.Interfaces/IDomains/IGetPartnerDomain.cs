using SWTI.Utils;
using SWTL.Models.Entities;

namespace SWTI.Interfaces.IDomains
{
    public interface IGetPartnerDomain
    {
        Task<(Partner, BaseResponse)> GetPartnerByCode(string code, CancellationToken cancellationToken);
    }
}
