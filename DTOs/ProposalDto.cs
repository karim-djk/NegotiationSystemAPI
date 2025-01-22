namespace NegotiationSystemAPI.DTOs
{
    public class ProposalDto
    {
        public int ProposalId { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatorName { get; set; }
        public string CreatorParty { get; set; }
        public string Message { get; set; }
        public bool IsCounterProposal { get; set; }
        public int? InitialProposalId { get; set; }

        public List<PaymentRatioDto> Payments { get; set; } = new List<PaymentRatioDto>();
    }
}
