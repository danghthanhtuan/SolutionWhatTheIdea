using MapsterMapper;
using Microsoft.Extensions.Logging;
using SWTI.Enums;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.ModelDapper.Categories;
using SWTL.Models.Requests.Categories;

namespace SWTI.Categories.Domain.Domain
{
    public class CreateCategoryDomain : ICreateCategoryDomain
    {
        private readonly ILogger<CreateCategoryDomain> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CreateCategoryDomain(ILogger<CreateCategoryDomain> logger
            , ICategoryRepository categoryRepository
            , IMapper mapper)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<(int, BaseResponse?)> CreateCategory(CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(CreateCategoryDomain)} {nameof(CreateCategory)} request = {request.Dump()}");
            if (request.CategoryParent > 0)
            {
                var (resCate, errCate) = await _categoryRepository.GetCategoryByID(request.CategoryParent, cancellationToken);
                if (errCate is not null)
                {
                    _logger.LogError($"{nameof(CreateCategoryDomain)} {nameof(CreateCategory)} errCate = {errCate.Dump()}");
                    return ((int)ErrorCode.SystemError, errCate);
                }

                if (resCate is null)
                {
                    _logger.LogError($"{nameof(CreateCategoryDomain)} {nameof(CreateCategory)} resCate = null with id categoryParent : {request.CategoryParent}");
                    return ((int)ErrorCode.SystemError, BaseResponseExt.Error((int)ErrorCode.BadRequest, "Không tìm thấy Category Parent"));
                }
            }

            var modelDapper = _mapper.Map<CreateCategoryDapper>(request);
            var (res, err) = await _categoryRepository.CreateCategory(modelDapper, cancellationToken);

            if (err is not null && err.HasError)
            {
                _logger.LogError($"{nameof(CreateCategoryDomain)} {nameof(CreateCategory)} modelDapper = {modelDapper.Dump()} error >> {err.Dump()} ");
                return (-1, err);
            }

            return (res, null);
        }
    }
}
