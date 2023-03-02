using SWTI.Utils;
using SWTL.Models.Requests;

namespace SWTI.Interfaces.IDomains
{
    public interface ICreateProductImageDomain
    {
        Task<(int, BaseResponse?)> CreateProductImages(CreateProductImagesRequest request, CancellationToken cancellationToken);
    }
}
