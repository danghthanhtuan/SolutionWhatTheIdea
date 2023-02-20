using Microsoft.AspNetCore.Mvc;
using SWTI.Interfaces.IDomains;
using SWTI.RedisProvider;
using SWTL.Models.Requests.Account;

namespace SWTI.Api.Test.Controllers
{
    [Route("api/v1/Test")]
    public class TestController : ControllerBase
    {
        private readonly ICreateAccountDomain _createAccountDomain;
        private readonly SWTIRedis _redisClient;
        
        public TestController(
            ICreateAccountDomain createAccountDomain
            , SWTIRedis redisClient)
        {
            _createAccountDomain = createAccountDomain;
            _redisClient= redisClient;
        }

        [HttpPost("test")]
        //[ServiceFilter(typeof(FarmVirtualAccountAuthFilterAttribute))]
        public async Task<IActionResult> TEST([FromBody] CreateAccountRequest req, CancellationToken cancellationToken)
        {
            await _createAccountDomain.CreateAccount(req, cancellationToken);
            return Ok();
        }

        [HttpPost("test-redis")]
        //[ServiceFilter(typeof(FarmVirtualAccountAuthFilterAttribute))]
        public async Task<IActionResult> TEST_Redis([FromBody] CreateAccountRequest req, CancellationToken cancellationToken)
        {
            _redisClient.RedisClient.SetValueStringFromKey("TEST", "tuanTEST", TimeSpan.FromMinutes(1));
            return Ok();
        }
    }
}
