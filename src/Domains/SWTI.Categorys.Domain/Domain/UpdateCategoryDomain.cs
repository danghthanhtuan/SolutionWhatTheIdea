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
    public class UpdateCategoryDomain : IUpdateCategoryDomain
    {
        private readonly ILogger<UpdateCategoryDomain> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public UpdateCategoryDomain(ILogger<UpdateCategoryDomain> logger
            , ICategoryRepository categoryRepository
            , IMapper mapper)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<(int, BaseResponse?)> UpdateCategory(UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(UpdateCategoryDomain)} {nameof(UpdateCategory)} request = {request.Dump()}");

            var (resCate, errCate) = await _categoryRepository.GetCategoryByID(request.CategoryId, cancellationToken);
            if (errCate is not null)
            {
                _logger.LogError($"{nameof(UpdateCategoryDomain)} {nameof(UpdateCategory)} errCate = {errCate.Dump()}");
                return ((int)ErrorCode.SystemError, errCate);
            }

            if (resCate is null)
            {
                _logger.LogError($"{nameof(UpdateCategoryDomain)} {nameof(UpdateCategory)} resCate = null with id categoryParent : {request.CategoryParent}");
                return ((int)ErrorCode.SystemError, BaseResponseExt.Error((int)ErrorCode.BadRequest, "Không tìm thấy Category"));
            }

            if (request.CategoryParent > 0)
            {
                (resCate, errCate) = await _categoryRepository.GetCategoryByID(request.CategoryParent, cancellationToken);
                if (errCate is not null)
                {
                    _logger.LogError($"{nameof(UpdateCategoryDomain)} {nameof(UpdateCategory)} errCate = {errCate.Dump()}");
                    return ((int)ErrorCode.SystemError, errCate);
                }

                if (resCate is null)
                {
                    _logger.LogError($"{nameof(UpdateCategoryDomain)} {nameof(UpdateCategory)} resCate = null with id categoryParent : {request.CategoryParent}");
                    return ((int)ErrorCode.SystemError, BaseResponseExt.Error((int)ErrorCode.BadRequest, "Không tìm thấy Category Parent"));
                }
            }

            var modelDapper = _mapper.Map<UpdateCategoryDapper>(request);
            var (res, err) = await _categoryRepository.UpdateCategory(modelDapper, cancellationToken);

            if (err is not null && err.HasError)
            {
                _logger.LogError($"{nameof(UpdateCategoryDomain)} {nameof(UpdateCategory)} modelDapper = {modelDapper.Dump()} error >> {err.Dump()} ");
                return (-1, err);
            }

            return (res, null);
        }
    }
}
