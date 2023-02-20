using SWTI.Utils;
using SWTL.Models.Requests.Partner;

namespace SWTI.Interfaces.IDomains
{
    public interface ICreatePartnerDomain
    {
        Task<(int, BaseResponse?)> CreatePartner(CreatePartnerRequest request, CancellationToken cancellationToken);
    }
}
