using SWTI.Utils;
using SWTL.Models.ModelDapper.Products;
using SWTL.Models.Requests.Products;

namespace SWTI.Interfaces.IRepositories
{
    public interface IProductRepository
    {
        public Task<(int, BaseResponse?)> CreateProduct(CreateProductDapper request, CancellationToken cancellationToken);
        
    }
}
