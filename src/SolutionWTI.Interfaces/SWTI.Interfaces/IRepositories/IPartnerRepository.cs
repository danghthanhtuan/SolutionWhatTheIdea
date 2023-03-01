using SWTI.Utils;
using SWTL.Models.Entities;
using SWTL.Models.ModelDapper;
using SWTL.Models.Requests.Partner;

namespace SWTI.Interfaces.IRepositories
{
    public interface IPartnerRepository
    {
        Task<(SWTL.Models.Entities.Partners, BaseResponse)> GetPartnerByCode(string code, CancellationToken cancellationToken);
        Task<(SWTL.Models.Entities.Partners, BaseResponse)> GetPartnerByID(int id, CancellationToken cancellationToken);
        Task<(IEnumerable<Partners>,int , BaseResponse)> GetPartnePaging(GetPartnerPagingRequest req, CancellationToken cancellationToken);


        Task<(int, BaseResponse)> CreatePartner(CreatePartnerDapper request, CancellationToken cancellationToken);

        Task<(int, BaseResponse)> UpdatePartner(UpdatePartnerDapper request, CancellationToken cancellationToken);
    }
}
