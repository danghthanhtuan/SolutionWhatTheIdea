using MapsterMapper;
using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.Entities;
using SWTL.Models.Requests.Partner;
using SWTL.Models.Requests;
using SWTL.Models.Requests.Products;

namespace SWTI.Products.Domain.Domain
{
    public class GetProductDomain : IGetProductDomain
    {
        private readonly ILogger<GetProductDomain> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetProductDomain(ILogger<GetProductDomain> logger
            , IProductRepository productRepository
            , IMapper mapper)
        {
            _logger = logger;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<(PagingResDto<SWTL.Models.Entities.Products>?, BaseResponse?)> GetProductPaging(GetProductPagingRequest req, CancellationToken cancellationToken)
        {
            req.SetValueDefault();
            var (data, totalRow, error) = await _productRepository.GetProductPaging(req, cancellationToken);
            if (error is not null && error.HasError)
            {
                return (null, error);
            }

            return (new PagingResDto<SWTL.Models.Entities.Products>()
            {
                Data = data,
                Page = req.Page,
                Total = totalRow,
                PageSize = req.PageSize
            }, null);
        }


        public async Task<(SWTL.Models.Entities.Products, BaseResponse)> GetProductByID(int id, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProductByID(id, cancellationToken);
        }

    }
}
