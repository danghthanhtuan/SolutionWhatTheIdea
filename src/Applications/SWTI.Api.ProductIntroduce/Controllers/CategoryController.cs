using Microsoft.AspNetCore.Mvc;
using SWTI.Interfaces.IDomains;
using SWTI.Utils;
using SWTL.Models.Requests.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWTI.Api.ProductIntroduce.Controllers
{
    //[ApiVersion("1.0")]
    [Route("api/v1/category")]
    [ApiController]
    public class CategoryController : CommonController
    {
        private readonly ICreateCategoryDomain _createCategoryDomain;
        private readonly IGetCategoryDomain _getCategoryDomain;
        private readonly IUpdateCategoryDomain _updateCategoryDomain;

        public CategoryController(ICreateCategoryDomain createCategoryDomain
            , IGetCategoryDomain getCategoryDomain
            , IUpdateCategoryDomain updateCategoryDomain)
        {
            _createCategoryDomain = createCategoryDomain;
            _getCategoryDomain = getCategoryDomain;
            _updateCategoryDomain = updateCategoryDomain;
        }

        [HttpGet("by-id")]
        public async Task<IActionResult> GetByID(int id, CancellationToken cancellationToken)
        {
            var (res, err) = await _getCategoryDomain.GetCategoryByID(id, cancellationToken);

            return BaseResponse(res, err);
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetPaging([FromQuery] GetCategoryPagingRequest req, CancellationToken cancellationToken)
        {
            var (res, err) = await _getCategoryDomain.GetCategoryPaging(req, cancellationToken);

            return BaseResponse(res, err);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePartner([FromBody] CreateCategoryRequest req, CancellationToken cancellationToken)
        {
            var (res, err) = await _createCategoryDomain.CreateCategory(req, cancellationToken);

            return BaseResponse(res, err);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartner([FromBody] UpdateCategoryRequest req, CancellationToken cancellationToken)
        {
            var (res, err) = await _updateCategoryDomain.UpdateCategory(req, cancellationToken);

            return BaseResponse(res, err);
        }
    }
}
