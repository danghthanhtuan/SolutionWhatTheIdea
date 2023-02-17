using Microsoft.AspNetCore.Mvc;
using SWTI.Interfaces.IDomains;
using SWTL.Models.Requests.Account;

namespace SWTI.Api.Test.Controllers
{
    public class TestController : ControllerBase
    {
        private readonly ICreateAccountDomain _createAccountDomain;

        public TestController(
            ICreateAccountDomain createAccountDomain)
        {
            _createAccountDomain = createAccountDomain;
        }

        [HttpPost("test")]
        //[ServiceFilter(typeof(FarmVirtualAccountAuthFilterAttribute))]
        public async Task<IActionResult> TEST([FromBody] CreateAccountRequest req, CancellationToken cancellationToken)
        {
            await _createAccountDomain.CreateAccount(req, cancellationToken);
            return Ok();
        }
    }
}
