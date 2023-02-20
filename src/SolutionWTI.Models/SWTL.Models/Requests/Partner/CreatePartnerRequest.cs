using Microsoft.AspNetCore.Http;

namespace SWTL.Models.Requests.Partner
{
    public class CreatePartnerRequest
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string UrlLogo { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }
}
