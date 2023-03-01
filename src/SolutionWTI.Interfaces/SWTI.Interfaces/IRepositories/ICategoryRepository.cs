using SWTI.Utils;
using SWTL.Models.Entities;
using SWTL.Models.ModelDapper.Categories;
using SWTL.Models.Requests.Categories;

namespace SWTI.Interfaces.IRepositories
{
    public interface ICategoryRepository
    {
        //Task<(Categories, BaseResponse)> GetCategoryByID(int id, CancellationToken cancellationToken);
        Task<(Categories, BaseResponse?)> GetCategoryByID(int id, CancellationToken cancellationToken);
        Task<(IEnumerable<Categories>?, int, BaseResponse?)> GetCategoryPaging(GetCategoryPagingRequest req, CancellationToken cancellationToken);

        Task<(int, BaseResponse?)> CreateCategory(CreateCategoryDapper request, CancellationToken cancellationToken);
        Task<(int, BaseResponse?)> UpdateCategory(UpdateCategoryDapper request, CancellationToken cancellationToken);
    }
}
