namespace SWTL.Models.Entities
{
    public class Partner
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string UrlLogo { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedUser { get; set; }
        public string CreatedUser { get; set; }
        public int Status { get; set; }
    }
}
