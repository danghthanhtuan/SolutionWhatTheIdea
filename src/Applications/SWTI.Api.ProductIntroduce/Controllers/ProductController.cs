using Microsoft.AspNetCore.Mvc;
using SWTI.Interfaces.IDomains;
using SWTI.Utils;
using SWTL.Models.Requests.Products;

namespace SWTI.Api.ProductIntroduce.Controllers
{
    [Route("api/v1/Product")]
    [ApiController]
    public class ProductController : CommonController
    {
        private readonly ICreateProductDomain _createProductDomain;
        private readonly IGetProductDomain _getProductDomain;
        private readonly IUpdateProductDomain _updateProductDomain;

        public ProductController(ICreateProductDomain createProductDomain
            , IGetProductDomain getProductDomain
            , IUpdateProductDomain updateProductDomain)
        {
            _createProductDomain = createProductDomain;
            _getProductDomain = getProductDomain;
            _updateProductDomain = updateProductDomain;
        }

        [HttpGet("by-id")]
        public async Task<IActionResult> GetByID(int id, CancellationToken cancellationToken)
        {
            var (res, err) = await _getProductDomain.GetProductByID(id, cancellationToken);

            return BaseResponse(res, err);
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetPaging([FromQuery] GetProductPagingRequest req, CancellationToken cancellationToken)
        {
            var (res, err) = await _getProductDomain.GetProductPaging(req, cancellationToken);

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
