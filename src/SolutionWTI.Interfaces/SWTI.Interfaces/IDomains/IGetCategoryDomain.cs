using SWTI.Utils;
using SWTL.Models.Entities;
using SWTL.Models.Requests;
using SWTL.Models.Requests.Categories;

namespace SWTI.Interfaces.IDomains
{
    public interface IGetCategoryDomain
    {
        public Task<(SWTL.Models.Entities.Categories?, BaseResponse?)> GetCategoryByID(int id, CancellationToken cancellationToken);
        public Task<(PagingResDto<Categories>?, BaseResponse?)> GetCategoryPaging(GetCategoryPagingRequest req, CancellationToken cancellationToken);
        
    }
}
