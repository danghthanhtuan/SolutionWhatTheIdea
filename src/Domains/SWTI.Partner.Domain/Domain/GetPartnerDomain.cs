using MapsterMapper;
using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;

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

        public async Task<(SWTL.Models.Entities.Partner, BaseResponse)> GetPartnerByCode(string code, CancellationToken cancellationToken)
        {
           return await _partnerRepository.GetPartnerByCode(code, cancellationToken);
        }
    }
}
