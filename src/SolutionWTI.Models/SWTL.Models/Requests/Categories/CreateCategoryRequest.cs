using System.ComponentModel.DataAnnotations;

namespace SWTL.Models.Requests.Categories
{
    public class CreateCategoryRequest
    {
        [Required]
        public string CategoryName { get; set; }
        public int CategoryParent { get; set; }
        public int SortOrder { get; set; }
    }
}
