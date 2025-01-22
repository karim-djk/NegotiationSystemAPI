namespace NegotiationSystemAPI.DTOs
{
    public class ItemDto
    {
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public decimal Price { get; set; }
        public bool IsShared { get; set; }
    }
}
