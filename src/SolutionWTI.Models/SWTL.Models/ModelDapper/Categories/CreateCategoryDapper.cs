namespace SWTL.Models.ModelDapper.Categories
{
    public class CreateCategoryDapper
    {
        public string CategoryName { get; set; }
        public int CategoryParent { get; set; }
        public int SortOrder { get; set; }
    }
}
