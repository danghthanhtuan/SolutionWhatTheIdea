namespace SWTL.Models.Entities
{
    public class Partners : DateTimeAction
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string UrlLogo { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
    }
}
