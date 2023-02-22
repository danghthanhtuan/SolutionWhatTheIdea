namespace SWTL.Models.ModelDapper
{
    public class UpdatePartnerDapper
    {
        public string Name { get; set; }
        public string UrlLogo { get; set; }
        public string Description { get; set; }
        public string UpdatedUser { get; set; } = "system";
        public int Status { get; set; }
        public int PartnerId { get; set; }
    }
}
