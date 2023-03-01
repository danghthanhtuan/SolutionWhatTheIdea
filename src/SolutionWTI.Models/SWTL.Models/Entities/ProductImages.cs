namespace SWTL.Models.Entities
{
    public class ProductImages : DateTimeAction
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public bool IsImageMain { get; set; }
        public string ImageUrl { get; set; }
    }
}
