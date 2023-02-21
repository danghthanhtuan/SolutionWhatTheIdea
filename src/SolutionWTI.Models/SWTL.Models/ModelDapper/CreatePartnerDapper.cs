namespace SWTL.Models.ModelDapper
{
    public class CreatePartnerDapper
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string UrlLogo { get; set; }
        public string Description { get; set; }
        public string CreatedUser { get; set; } = "system";
    }
}
