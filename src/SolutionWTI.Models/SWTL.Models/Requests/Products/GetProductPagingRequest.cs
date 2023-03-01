namespace SWTL.Models.Requests.Products
{
    public class GetProductPagingRequest : Paging
    {
        public string ProductName { get;set; }
        public int CategoryId { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
