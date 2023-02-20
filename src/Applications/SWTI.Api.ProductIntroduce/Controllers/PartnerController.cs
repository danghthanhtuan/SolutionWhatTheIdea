using Microsoft.AspNetCore.Mvc;
using SWTI.Interfaces.IDomains;
using SWTI.Utils;
using SWTL.Models.Requests.Partner;

namespace SWTI.Api.ProductIntroduce.Controllers
{
    //[ApiVersion("1.0")]
    [Route("api/v1/ProductIntroduceTest")]
    [ApiController]
    public class PartnerController : CommonController
    {
        private readonly ICreatePartnerDomain _createPartnerDomain;

        public PartnerController(ICreatePartnerDomain createPartnerDomain)
        {
            _createPartnerDomain = createPartnerDomain;
        }

        [HttpPost]
        //[ServiceFilter(typeof(FarmVirtualAccountAuthFilterAttribute))]
        public async Task<IActionResult> TEST([FromBody] CreatePartnerRequest req, CancellationToken cancellationToken)
        {
            var (res, err) = await _createPartnerDomain.CreatePartner(req, cancellationToken);

            return BaseResponse(res, err);
        }
    }
}
