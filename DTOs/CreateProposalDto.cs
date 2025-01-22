namespace NegotiationSystemAPI.DTOs
{
    public class CreateProposalDto
    {
        public int ItemId { get; set; }
        public string? Message { get; set; }
        public bool IsPercentageBased { get; set; }
        public List<CreatePaymentRatioDto> Payments { get; set; } = new List<CreatePaymentRatioDto>();
    }
}
