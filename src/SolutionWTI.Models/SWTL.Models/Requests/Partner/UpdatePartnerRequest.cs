using Microsoft.AspNetCore.Http;

namespace SWTL.Models.Requests.Partner
{
    public class UpdatePartnerRequest
    {
        public int PartnerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public int Status { get; set; }
    }
}
