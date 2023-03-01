namespace SWTL.Models.Requests.Categories
{
    public class UpdateCategoryRequest
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int CategoryParent { get; set; }
        public int SortOrder { get; set; }
        public int Status { get; set; }
    }
}
