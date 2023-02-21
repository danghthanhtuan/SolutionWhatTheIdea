using SWTI.Utils;
using SWTL.Models.ModelDapper;

namespace SWTI.Interfaces.IRepositories
{
    public interface IPartnerRepository
    {
        Task<(SWTL.Models.Entities.Partner, BaseResponse)> GetPartnerByCode(string code, CancellationToken cancellationToken);
        Task<(SWTL.Models.Entities.Partner, BaseResponse)> GetPartnerByID(int id, CancellationToken cancellationToken);

        Task<(int, BaseResponse)> CreatePartner(CreatePartnerDapper request, CancellationToken cancellationToken);

        Task<(int, BaseResponse)> UpdatePartner(UpdatePartnerDapper request, CancellationToken cancellationToken);
    }
}
