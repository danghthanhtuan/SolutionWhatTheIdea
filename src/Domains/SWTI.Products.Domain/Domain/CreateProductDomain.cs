using MapsterMapper;
using Microsoft.Extensions.Logging;
using SWTI.Enums;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.ModelDapper.Products;
using SWTL.Models.Requests.Products;

namespace SWTI.Products.Domain.Domain
{
    public class CreateProductDomain : ICreateProductDomain
    {
        private readonly ILogger<CreateProductDomain> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CreateProductDomain(ILogger<CreateProductDomain> logger
            , IProductRepository productRepository
            , ICategoryRepository categoryRepository
            , IMapper mapper)
        {
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<(int, BaseResponse?)> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(CreateProductDomain)} {nameof(CreateProduct)} request = {request.Dump()}");
            if (request.CategoryId > 0)
            {
                var (resCate, errCate) = await _categoryRepository.GetCategoryByID(request.CategoryId, cancellationToken);
                if (errCate is not null)
                {
                    _logger.LogError($"{nameof(CreateProductDomain)} {nameof(CreateProduct)} errCate = {errCate.Dump()}");
                    return ((int)ErrorCode.SystemError, errCate);
                }

                if (resCate is null)
                {
                    _logger.LogError($"{nameof(CreateProductDomain)} {nameof(CreateProduct)} resCate = null with id categoryParent : {request.CategoryId}");
                    return ((int)ErrorCode.SystemError, BaseResponseExt.Error((int)ErrorCode.BadRequest, "Không tìm thấy Category Parent"));
                }
            }

            var modelDapper = _mapper.Map<CreateProductDapper>(request);
            var (res, err) = await _productRepository.CreateProduct(modelDapper, cancellationToken);

            if (err is not null && err.HasError)
            {
                _logger.LogError($"{nameof(CreateProductDomain)} {nameof(CreateProduct)} modelDapper = {modelDapper.Dump()} error >> {err.Dump()} ");
                return (-1, err);
            }

            return (res, null);
        }
    }
}
