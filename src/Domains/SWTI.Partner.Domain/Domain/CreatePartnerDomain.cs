using MapsterMapper;
using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.ModelDapper;
using SWTL.Models.Requests.Partner;

namespace SWTI.Partner.Domain.Domain
{
    public class CreatePartnerDomain : ICreatePartnerDomain
    {
        private readonly ILogger<CreatePartnerDomain> _logger;
        private readonly IUploadFileToServerDomain _uploadFileDomain;
        private readonly IPartnerRepository _partnerRepository;
        private readonly IMapper _mapper;

        public CreatePartnerDomain(ILogger<CreatePartnerDomain> logger
            , IPartnerRepository partnerRepository
            , IUploadFileToServerDomain uploadFileDomain
            , IMapper mapper)
        {
            _logger = logger;
            _partnerRepository = partnerRepository;
            _uploadFileDomain = uploadFileDomain;
            _mapper = mapper;
        }

        public async Task<(int, BaseResponse?)> CreatePartner(CreatePartnerRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"CreatePartnerDomain CreatePartner request = {request.Dump()}");

            var(partner, errorGet) = await _partnerRepository.GetPartnerByCode(request.Code, cancellationToken);
            if(partner != null)
            {
                _logger.LogInformation($"CreatePartnerDomain CreatePartner GetPartnerByCode is not null >> {partner.Dump()}");
                return (-1, BaseResponseExt.Error(400, "Code Partner đã tồn tại!"));
            }

            if (errorGet is not null)
            {
                _logger.LogInformation($"CreatePartnerDomain CreatePartner GetPartnerByCode errorGet = {errorGet.Dump()}");
                return (-1, errorGet);
            }

            var (urlLogo, errorUpload) = _uploadFileDomain.UploadFileToServer(request.Image, Enums.FolderUploadEnum.Partner, request.Code, cancellationToken);
            if (errorUpload is not null)
            {
                _logger.LogInformation($"CreatePartnerDomain CreatePartner errorUpload = {errorUpload.Dump()}");
                return (-1, errorUpload);
            }

            var modelDapper = _mapper.Map<CreatePartnerDapper>(request);
            modelDapper.UrlLogo = urlLogo;
            var (res, err) = await _partnerRepository.CreatePartner(modelDapper, cancellationToken);

            if(err is not null && err.HasError)
            {
                _logger.LogError($"CreatePartnerDomain CreatePartner modelDapper = {modelDapper.Dump()} error >> {err.Dump()} ");
                return (-1, err);
            }

            return (res, null);
        }
    }
}
