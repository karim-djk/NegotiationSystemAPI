using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NegotiationSystemAPI.Models
{
    public class Proposal
    {
        [Key]
        public int ProposalId { get; set; }
        public int ItemId { get; set; }
        public DateTime CreationDate { get; set; }
        public int CreatedBy { get; set; }
        public string Message { get; set; }
        public bool IsCounterProposal { get; set; }
        public int? InitialProposalId { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public User User { get; set; } = null!;

        [ForeignKey(nameof(InitialProposalId))]
        public Proposal? InitialProposal { get; set; }

        [ForeignKey(nameof(ItemId))]
        public Item Item { get; set; } = null!;

        public List<PaymentRatio> PaymentRatios { get; set; } = new List<PaymentRatio>();
    }
}
