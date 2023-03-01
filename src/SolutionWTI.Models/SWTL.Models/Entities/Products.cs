namespace SWTL.Models.Entities
{
    public class Products : DateTimeAction
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string CategoryId { get; set; }
        public string Description { get; set; }
        public int PartnerID { get; set; }
        public int IsNew { get; set; }
        public int IsHot { get; set; }
        public int ViewCount { get; set; }
        public string Content { get; set; }
        public decimal Price { get; set; }
        public decimal PromotionPrice { get; set; }
        public string Video { get; set; }
        public int Status { get; set; }
        public string SeoAlias { get; set; }
    }
}