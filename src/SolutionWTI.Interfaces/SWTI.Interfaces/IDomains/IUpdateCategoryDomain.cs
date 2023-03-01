using SWTI.Utils;
using SWTL.Models.Requests.Categories;

namespace SWTI.Interfaces.IDomains
{
    public interface IUpdateCategoryDomain
    {
        public Task<(int, BaseResponse?)> UpdateCategory(UpdateCategoryRequest request, CancellationToken cancellationToken);
    }
}
