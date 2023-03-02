using SWTI.Utils;
using SWTL.Models.Requests.Products;

namespace SWTI.Interfaces.IDomains
{
    public interface IUpdateProductDomain
    {
        Task<(int, BaseResponse?)> UpdateProduct(UpdateProductRequest request, CancellationToken cancellationToken);
    }
}
