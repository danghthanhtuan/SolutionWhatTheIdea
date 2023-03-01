using SWTI.Utils;
using SWTL.Models.ModelDapper.Products;
using SWTL.Models.Requests.Products;

namespace SWTI.Interfaces.IRepositories
{
    public interface IProductRepository
    {
        public Task<(int, BaseResponse?)> CreateProduct(CreateProductDapper request, CancellationToken cancellationToken);
        public Task<(IEnumerable<SWTL.Models.Entities.Products>, int, BaseResponse)> GetProductPaging(GetProductPagingRequest req, CancellationToken cancellationToken);
        public Task<(SWTL.Models.Entities.Products?, BaseResponse?)> GetProductByID(int id, CancellationToken cancellationToken);
    }
}
