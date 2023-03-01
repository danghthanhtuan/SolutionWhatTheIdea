using SWTI.Utils;
using SWTL.Models.Entities;
using SWTL.Models.Requests;
using SWTL.Models.Requests.Products;

namespace SWTI.Interfaces.IDomains
{
    public interface IGetProductDomain
    {
        public Task<(SWTL.Models.Entities.Products, BaseResponse)> GetProductByID(int id, CancellationToken cancellationToken);
        public Task<(PagingResDto<Products>?, BaseResponse?)> GetProductPaging(GetProductPagingRequest req, CancellationToken cancellationToken);
    }
}
