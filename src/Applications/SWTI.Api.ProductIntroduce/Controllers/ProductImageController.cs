using Microsoft.AspNetCore.Mvc;
using SWTI.Interfaces.IDomains;
using SWTI.Utils;
using SWTL.Models.Requests.Products;

namespace SWTI.Api.ProductIntroduce.Controllers
{
    [Route("api/v1/Product")]
    [ApiController]
    public class ProductImageController : CommonController
    {
        private readonly ICreateProductImageDomain _createProductImageDomain;
        private readonly IGetProductImageDomain _getProductImageDomain;
        private readonly IUpdateProductImageDomain _updateProductImageDomain;

        public ProductImageController(ICreateProductImageDomain createProductImageDomain
            , IGetProductImageDomain getProductImageDomain
            , IUpdateProductImageDomain updateProductImageDomain)
        {
            _createProductImageDomain = createProductImageDomain;
            _getProductImageDomain = getProductImageDomain;
            _updateProductImageDomain = updateProductImageDomain;
        }

        [HttpGet("by-id")]
        public async Task<IActionResult> GetByID(int id, CancellationToken cancellationToken)
        {
            var (res, err) = await _getProductImageDomain.GetProductByID(id, cancellationToken);

            return BaseResponse(res, err);
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetPaging([FromQuery] GetProductPagingRequest req, CancellationToken cancellationToken)
        {
            var (res, err) = await _getProductImageDomain.GetProductPaging(req, cancellationToken);

            return BaseResponse(res, err);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePartner([FromBody] CreateProductRequest req, CancellationToken cancellationToken)
        {
            var (res, err) = await _createProductDomain.CreateProduct(req, cancellationToken);

            return BaseResponse(res, err);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartner([FromBody] UpdateProductRequest req, CancellationToken cancellationToken)
        {
            var (res, err) = await _updateProductDomain.UpdateProduct(req, cancellationToken);

            return BaseResponse(res, err);
        }
    }
}
