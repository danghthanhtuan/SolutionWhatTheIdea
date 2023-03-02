using MapsterMapper;
using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.Requests.Products;

namespace SWTI.Products.Domain.Domain
{
    public class UpdateProductDomain : IUpdateProductDomain
    {
        private readonly ILogger<UpdateProductDomain> _logger;
        private readonly IUploadFileToServerDomain _uploadFileDomain;
        private readonly IPartnerRepository _partnerRepository;
        private readonly IMapper _mapper;

        public UpdateProductDomain(ILogger<UpdateProductDomain> logger
            , IPartnerRepository partnerRepository
            , IUploadFileToServerDomain uploadFileDomain
            , IMapper mapper)
        {
            _logger = logger;
            _partnerRepository = partnerRepository;
            _uploadFileDomain = uploadFileDomain;
            _mapper = mapper;
        }
        public Task<(int, BaseResponse?)> UpdateProduct(UpdateProductRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
