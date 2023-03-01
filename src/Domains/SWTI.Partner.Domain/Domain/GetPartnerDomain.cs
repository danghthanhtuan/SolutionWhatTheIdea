using MapsterMapper;
using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.Entities;
using SWTL.Models.Requests;
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

        public async Task<(PagingResDto<Partners>?, BaseResponse?)> GetPartnePaging(GetPartnerPagingRequest req, CancellationToken cancellationToken)
        {
            req.SetValueDefault();
            var (data, totalRow, error) = await _partnerRepository.GetPartnePaging(req, cancellationToken);
            if (error is not null && error.HasError)
            {
                return (null, error);
            }

            return (new PagingResDto<Partners>()
            {
                Data = data,
                Page = req.Page,
                Total = totalRow,
                PageSize = req.PageSize
            }, null);
        }

        public async Task<(SWTL.Models.Entities.Partners, BaseResponse)> GetPartnerByCode(string code, CancellationToken cancellationToken)
        {
            return await _partnerRepository.GetPartnerByCode(code, cancellationToken);
        }
    }
}
