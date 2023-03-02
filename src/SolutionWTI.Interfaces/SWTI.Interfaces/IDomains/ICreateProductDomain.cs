using SWTI.Utils;
using SWTL.Models.Requests.Products;

namespace SWTI.Interfaces.IDomains
{
    public interface ICreateProductDomain
    {
        Task<(int, BaseResponse?)> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken);
    }
}
