using Microsoft.AspNetCore.Mvc;
using SWTI.Interfaces.IDomains;
using SWTI.Utils;
using SWTL.Models.Requests.Partner;

namespace SWTI.Api.ProductIntroduce.Controllers
{
    //[ApiVersion("1.0")]
    [Route("api/v1/partner")]
    [ApiController]
    public class PartnerController : CommonController
    {
        private readonly ICreatePartnerDomain _createPartnerDomain;
        private readonly IGetPartnerDomain _getPartnerDomain;
        private readonly IUpdatePartnerDomain _updatePartnerDomain;

        public PartnerController(ICreatePartnerDomain createPartnerDomain
            , IGetPartnerDomain getPartnerDomain
            , IUpdatePartnerDomain updatePartnerDomain            )
        {
            _createPartnerDomain = createPartnerDomain;
            _getPartnerDomain = getPartnerDomain;
            _updatePartnerDomain = updatePartnerDomain;
        }

        [HttpGet("by-code")]
        public async Task<IActionResult> GetByCode(string code, CancellationToken cancellationToken)
        {
            var (res, err) = await _getPartnerDomain.GetPartnerByCode(code, cancellationToken);

            return BaseResponse(res, err);
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetPaging([FromQuery]GetPartnerPagingRequest req, CancellationToken cancellationToken)
        {
            var (res, err) = await _getPartnerDomain.GetPartnePaging(req, cancellationToken);

            return BaseResponse(res, err);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePartner([FromForm] CreatePartnerRequest req, CancellationToken cancellationToken)
        {
            var (res, err) = await _createPartnerDomain.CreatePartner(req, cancellationToken);

            return BaseResponse(res, err);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartner([FromForm] UpdatePartnerRequest req, CancellationToken cancellationToken)
        {
            var (res, err) = await _updatePartnerDomain.UpdatePartner(req, cancellationToken);

            return BaseResponse(res, err);
        }
    }
}
