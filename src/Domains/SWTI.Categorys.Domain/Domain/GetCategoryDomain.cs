using MapsterMapper;
using Microsoft.Extensions.Logging;
using SWTI.Enums;
using SWTI.Interfaces.IDomains;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.Requests;
using SWTL.Models.Requests.Categories;

namespace SWTI.Categories.Domain.Domain
{
    public class GetCategoryDomain : IGetCategoryDomain
    {
        private readonly ILogger<GetCategoryDomain> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetCategoryDomain(ILogger<GetCategoryDomain> logger
            , ICategoryRepository categoryRepository
            , IMapper mapper)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<(SWTL.Models.Entities.Categories?, BaseResponse?)> GetCategoryByID(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(GetCategoryDomain)} >> {nameof(GetCategoryByID)} >> id : {id}");
            var (result, error) = await _categoryRepository.GetCategoryByID(id, cancellationToken);
            if(error != null)
            {
                return (null, error);
            }
            if(result is null)
                return (null, BaseResponseExt.Error((int)ErrorCode.BadRequest, "Không tìm thấy Category"));

            return (result, null);
        }

        public async Task<(PagingResDto<SWTL.Models.Entities.Categories>?, BaseResponse?)> GetCategoryPaging(GetCategoryPagingRequest req, CancellationToken cancellationToken)
        {
            req.SetValueDefault();
            var (data, totalRow, error) = await _categoryRepository.GetCategoryPaging(req, cancellationToken);
            if (error is not null && error.HasError)
            {
                return (null, error);
            }

            return (new PagingResDto<SWTL.Models.Entities.Categories>()
            {
                Data = data,
                Page = req.Page,
                Total = totalRow,
                PageSize = req.PageSize
            }, null);
        }
    }
}
