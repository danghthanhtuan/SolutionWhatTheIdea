using SWTI.Utils;
using SWTL.Models.Requests.Partner;

namespace SWTI.Interfaces.IDomains
{
    public interface IUpdatePartnerDomain
    {
        Task<(int, BaseResponse?)> UpdatePartner(UpdatePartnerRequest request, CancellationToken cancellationToken);
    }
}
