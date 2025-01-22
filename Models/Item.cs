using System.ComponentModel.DataAnnotations;

namespace NegotiationSystemAPI.Models
{
    public class Item
    {
        [Key]
        public int ItemId { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public decimal Price { get; set; }
        public List<Party> Parties { get; set; } = new List<Party>();
        public List<Proposal> Proposals { get; set; } = new List<Proposal>();
    }
}
