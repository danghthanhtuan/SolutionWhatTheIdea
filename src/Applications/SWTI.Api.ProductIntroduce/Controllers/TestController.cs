using Microsoft.AspNetCore.Mvc;

namespace SWTI.Api.ProductIntroduce.Controllers
{
    public class TestController : ControllerBase
    {
        public TestController()
        {

        }

        [HttpPost("test")]
        //[ServiceFilter(typeof(FarmVirtualAccountAuthFilterAttribute))]
        public async Task<IActionResult> TEST([FromBody] CancellationToken cancellationToken)
        {

            return Ok();
        }
    }
}
