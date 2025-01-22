using NegotiationSystemAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NegotiationSystemAPI.Models
{
    public class PaymentRatio
    {
        [Key]
        public int PaymentRatioId { get; set; }
        public int ProposalId { get; set; }
        public int PartyId { get; set; }
        public decimal Amount { get; set; }
        public ProposalStatus Status { get; set; }
        public int? StatusedBy { get; set; }

        [ForeignKey(nameof(StatusedBy))]
        public User? User { get; set; }

        [ForeignKey(nameof(ProposalId))]
        public Proposal Proposal { get; set; } = null!;

        [ForeignKey(nameof(PartyId))]
        public Party Party { get; set; } = null!;
    }
}
