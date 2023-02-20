using SWTI.Utils;
using SWTL.Models.Requests.Partner;

namespace SWTI.Interfaces.IRepositories
{
    public interface IPartnerRepository
    {
        Task<(int, BaseResponse)> CreatePartner(CreatePartnerRequest request, CancellationToken cancellationToken);
    }
}
