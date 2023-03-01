using SWTI.Utils;
using SWTL.Models.Entities;
using SWTL.Models.Requests;
using SWTL.Models.Requests.Partner;

namespace SWTI.Interfaces.IDomains
{
    public interface IGetPartnerDomain
    {
        Task<(Partners, BaseResponse)> GetPartnerByCode(string code, CancellationToken cancellationToken);
        Task<(PagingResDto<Partners>, BaseResponse)> GetPartnePaging(GetPartnerPagingRequest req, CancellationToken cancellationToken);
    }
}
