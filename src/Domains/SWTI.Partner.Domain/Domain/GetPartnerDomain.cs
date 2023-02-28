using MapsterMapper;
using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.Entities;
using SWTL.Models.Requests.Partner;

namespace SWTI.Partner.Domain.Domain
{
    public class GetPartnerDomain : IGetPartnerDomain
    {
        private readonly ILogger<GetPartnerDomain> _logger;
        private readonly IPartnerRepository _partnerRepository;
        private readonly IMapper _mapper;

        public GetPartnerDomain(ILogger<GetPartnerDomain> logger
            , IPartnerRepository partnerRepository
            , IMapper mapper)
        {
            _logger = logger;
            _partnerRepository = partnerRepository;
            _mapper = mapper;
        }

        public Task<(IEnumerable<Partners>, BaseResponse)> GetPartnePaging(GetPartnerPagingRequest req, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<(SWTL.Models.Entities.Partners, BaseResponse)> GetPartnerByCode(string code, CancellationToken cancellationToken)
        {
           return await _partnerRepository.GetPartnerByCode(code, cancellationToken);
        }
    }
}
