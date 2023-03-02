using SWTI.Interfaces.IDomains;
using SWTI.Utils;
using SWTL.Models.Requests;

namespace SWTI.ProductImage.Domain.Domain
{
    public class CreateProductImageDomain : ICreateProductImageDomain
    {
        public Task<(int, BaseResponse?)> CreateProductImages(CreateProductImagesRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
