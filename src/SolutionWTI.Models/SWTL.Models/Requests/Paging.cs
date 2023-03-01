namespace SWTL.Models.Requests
{
    public class Paging
    {
        public int PageSize
        {
            get; set;
        }
        public int Page
        {
            get; set;
        }

        public void SetValueDefault()
        {
            if(PageSize < 1)
            {
                PageSize = 10;
            }
            if(Page < 1)
            {
                Page = 1;
            }
        }
    }
}
