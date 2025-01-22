using System.ComponentModel.DataAnnotations;

namespace NegotiationSystemAPI.Models
{
    public class Party
    {
        [Key]
        public int PartyId { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; } = new List<User>();
        public List<Item> Items { get; set; } = new List<Item>();
    }
}
