using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using SWTL.Models.ConsumerRequest;

namespace SWTI.Api.ProductIntroduce.Controllers
{
    //[ApiVersion("1.0")]
    [Route("api/v1/ProductIntroduceTest")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IBus _bus;
        public TestController(IBus bus)
        {
            _bus = bus;
        }

        [HttpPost("test1")]
        //[ServiceFilter(typeof(FarmVirtualAccountAuthFilterAttribute))]
        public async Task<IActionResult> TEST([FromBody] CancellationToken cancellationToken)
        {
            await _bus.PubSub.PublishAsync(new TestConsumerRequest()
            {

            }, cancellationToken);
            return Ok();
        }
    }
}
