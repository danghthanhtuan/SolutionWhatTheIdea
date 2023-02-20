using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.Requests.Partner;

namespace SWTI.Partner.Domain.Domain
{
    public class CreatePartnerDomain : ICreatePartnerDomain
    {
        private readonly ILogger<CreatePartnerDomain> _logger;
        private readonly IPartnerRepository _partnerRepository;

        public CreatePartnerDomain(ILogger<CreatePartnerDomain>  logger
            , IPartnerRepository partnerRepository)
        {
            _logger = logger;
            _partnerRepository = partnerRepository;
        }

        public async Task<(int, BaseResponse?)> CreatePartner(CreatePartnerRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"CreatePartnerDomain CreatePartner request = {request.Dump()}");
            var (res, err) = await _partnerRepository.CreatePartner(request, cancellationToken);

            if(err is not null && err.HasError)
            {
                _logger.LogError($"CreatePartnerDomain CreatePartner error = {request.Dump()} error >> {err.Dump()} ");
                return (-1, err);
            }

            return (res, null);
        }
    }
}
