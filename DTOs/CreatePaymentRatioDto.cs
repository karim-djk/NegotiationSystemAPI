namespace NegotiationSystemAPI.DTOs
{
    public class CreatePaymentRatioDto
    {
        public int PartyId { get; set; }
        public decimal? Amount { get; set; }
        public int? Percentage { get; set; }
    }
}
