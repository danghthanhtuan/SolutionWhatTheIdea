namespace SWTL.Models.Requests.Partner
{
    public class GetPartnerPagingRequest : Paging
    {
        public GetPartnerPagingRequest() { }
        public string Name { get; set; }
        public string Code { get; set;}
        public string Description { get; set;}
        public string Phone { get; set;}
        public DateTime? DateFrom { get; set;}
        public DateTime? DateTo { get; set;}
    }
}
