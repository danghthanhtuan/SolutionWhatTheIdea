using MapsterMapper;
using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.ModelDapper;
using SWTL.Models.Requests.Partner;

namespace SWTI.Partner.Domain.Domain
{
    public class UpdatePartnerDomain : IUpdatePartnerDomain
    {
        private readonly ILogger<UpdatePartnerDomain> _logger;
        private readonly IUploadFileToServerDomain _uploadFileDomain;
        private readonly IPartnerRepository _partnerRepository;
        private readonly IMapper _mapper;

        public UpdatePartnerDomain(ILogger<UpdatePartnerDomain> logger
            , IPartnerRepository partnerRepository
            , IUploadFileToServerDomain uploadFileDomain
            , IMapper mapper)
        {
            _logger = logger;
            _partnerRepository = partnerRepository;
            _uploadFileDomain = uploadFileDomain;
            _mapper = mapper;
        }

        public async Task<(int, BaseResponse?)> UpdatePartner(UpdatePartnerRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($" {nameof(UpdatePartnerDomain)} {nameof(UpdatePartner)} request = {request.Dump()}");

            var (partner, errorGet) = await _partnerRepository.GetPartnerByID(request.PartnerId, cancellationToken);
            if (partner == null)
            {
                _logger.LogInformation($" {nameof(UpdatePartnerDomain)} {nameof(UpdatePartner)} GetPartnerByID is null >> {partner.Dump()}");
                return (-1, BaseResponseExt.Error(400, "Partner không tồn tại!"));
            }

            if (errorGet is not null)
            {
                _logger.LogInformation($" {nameof(UpdatePartnerDomain)} {nameof(UpdatePartner)} GetPartnerByID errorGet = {errorGet.Dump()}");
                return (-1, errorGet);
            }

            var (urlLogo, errorUpload) = _uploadFileDomain.DeleteAndUploadFileToServer(request.Image, Enums.FolderUploadEnum.Partner, partner.Code, partner.UrlLogo, cancellationToken);
            if (errorUpload is not null)
            {
                _logger.LogInformation($" {nameof(UpdatePartnerDomain)} {nameof(UpdatePartner)} errorUpload = {errorUpload.Dump()}");
                return (-1, errorUpload);
            }

            var modelDapper = _mapper.Map<UpdatePartnerDapper>(request);
            modelDapper.UrlLogo = urlLogo;
            var (res, err) = await _partnerRepository.UpdatePartner(modelDapper, cancellationToken);

            if (err is not null && err.HasError)
            {
                _logger.LogError($" {nameof(UpdatePartnerDomain)} {nameof(UpdatePartner)} modelDapper = {modelDapper.Dump()} error >> {err.Dump()} ");
                return (-1, err);
            }

            return (res, null);
        }
    }
}
