using SWTI.Utils;
using SWTL.Models.Requests.Categories;

namespace SWTI.Interfaces.IDomains
{
    public interface ICreateCategoryDomain
    {
        public Task<(int, BaseResponse?)> CreateCategory(CreateCategoryRequest request, CancellationToken cancellationToken);
    }
}
