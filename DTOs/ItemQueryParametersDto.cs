namespace NegotiationSystemAPI.DTOs
{
    public class ItemQueryParametersDto
    {
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsShared { get; set; }
        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; }
    }
}
