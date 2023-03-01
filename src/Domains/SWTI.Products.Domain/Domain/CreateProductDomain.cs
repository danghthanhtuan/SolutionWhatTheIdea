using MapsterMapper;
using Microsoft.Extensions.Logging;
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
        private readonly IMapper _mapper;

        public CreateProductDomain(ILogger<CreateProductDomain> logger
            , IProductRepository productRepository
            , IMapper mapper)
        {
            _logger = logger;
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<(int, BaseResponse?)> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(CreateProductDomain)} {nameof(CreateProduct)} request = {request.Dump()}");
            //if (request.CategoryParent > 0)
            //{
            //    var (resCate, errCate) = await _categoryRepository.GetCategoryByID(request.CategoryParent, cancellationToken);
            //    if (errCate is not null)
            //    {
            //        _logger.LogError($"{nameof(CreateProductDomain)} {nameof(CreateCategory)} errCate = {errCate.Dump()}");
            //        return ((int)ErrorCode.SystemError, errCate);
            //    }

            //    if (resCate is null)
            //    {
            //        _logger.LogError($"{nameof(CreateProductDomain)} {nameof(CreateCategory)} resCate = null with id categoryParent : {request.CategoryParent}");
            //        return ((int)ErrorCode.SystemError, BaseResponseExt.Error((int)ErrorCode.BadRequest, "Không tìm thấy Category Parent"));
            //    }
            //}

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
