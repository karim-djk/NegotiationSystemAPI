namespace NegotiationSystemAPI.DTOs
{
    public class PaymentRatioDto
    {
        public int PaymentRatioId { get; set; }
        public int PartyId { get; set; }
        public string PartyName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string ReviewerName { get; set; }
        public string ReviewerParty { get; set; }
    }
}
