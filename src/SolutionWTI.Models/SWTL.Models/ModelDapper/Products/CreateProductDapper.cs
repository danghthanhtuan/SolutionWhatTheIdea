namespace SWTL.Models.ModelDapper.Products
{
    public class CreateProductDapper 
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public int PartnerID { get; set; }
        public int IsNew { get; set; }
        public int IsHot { get; set; }
        public string Content { get; set; }
        public decimal Price { get; set; }
        public decimal PromotionPrice { get; set; }
        public string Video { get; set; }
    }
}
