namespace SWTL.Models.Requests
{
    public class PagingResDto<T>
    {
        public PagingResDto(int total, int pageSize, int page, decimal totalAmount, IEnumerable<T> data)
        {
            this.Data = data;
            this.Total = total;
            this.PageSize = pageSize;
            this.Page = page;
            this.TotalAmount = totalAmount;
        }
        public PagingResDto()
        {
        }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
        public decimal TotalAmount { get; set; }
        public IEnumerable<T> Data { get; set; }

        public int TotalPage => PageSize <= 0 ? 0 : ((Total - 1) / PageSize) + 1;
    }
}
