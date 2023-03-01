namespace SWTL.Models.Entities
{
    public class Categories : DateTimeAction
    {
        public int ID { get; set; }
        public string CategoryName { get; set; }     
        public int Status { get; set; }
    }
}
