using NegotiationSystemAPI.Enums;
using NegotiationSystemAPI.Models;

namespace NegotiationSystemAPI.Data
{
    public class DatabaseSeederService
    {
        private readonly ApplicationDbContext _context;

        public DatabaseSeederService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            if (_context.Parties.Any())
            {
                // Database is already seeded.
                return;
            }

            // Create parties.
            var partyA = new Party { Name = "Party A" };
            var partyB = new Party { Name = "Party B" };
            var partyC = new Party { Name = "Party C" };

            // Create users.
            var user1 = new User { UserName = $"{Environment.UserDomainName}\\{Environment.UserName}", FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Party = partyA };
            var user2 = new User { UserName = "user2", FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Party = partyB };
            var user3 = new User { UserName = "user3", FirstName = "Alice", LastName = "Johnson", Email = "alice.johnson@example.com", Party = partyC };

            // Create items.
            var item1 = new Item
            {
                Name = "Item 1",
                CreationDate = DateTime.UtcNow.AddDays(-10),
                Price = 1000.00m,
                Parties = new List<Party> { partyA, partyB }
            };

            var item2 = new Item
            {
                Name = "Item 2",
                CreationDate = DateTime.UtcNow.AddDays(-5),
                Price = 2000.00m,
                Parties = new List<Party> { partyB, partyC }
            };

            var item3 = new Item
            {
                Name = "Item 3",
                CreationDate = DateTime.UtcNow.AddDays(-2),
                Price = 500.00m,
                Parties = new List<Party> { partyC, partyA }
            };

            // Create proposals.
            var proposal1 = new Proposal
            {
                Item = item1,
                CreationDate = DateTime.UtcNow.AddDays(-7),
                CreatedBy = user1.UserId,
                User = user1,
                Message = "Initial proposal for Item 1",
                IsCounterProposal = false
            };

            var proposal2 = new Proposal
            {
                Item = item2,
                CreationDate = DateTime.UtcNow.AddDays(-3),
                CreatedBy = user2.UserId,
                User = user2,
                Message = "Initial proposal for Item 2",
                IsCounterProposal = false
            };

            // Create counter proposals.
            var counterProposal1 = new Proposal
            {
                Item = item1,
                CreationDate = DateTime.UtcNow.AddDays(-2),
                CreatedBy = user3.UserId,
                User = user3,
                Message = "Counter proposal for Item 1",
                IsCounterProposal = true,
                InitialProposal = proposal1
            };

            // Create payment ratios.
            var paymentRatio1 = new PaymentRatio
            {
                Proposal = proposal1,
                Party = partyA,
                Amount = 600.00m,
                Status = ProposalStatus.Pending
            };

            var paymentRatio2 = new PaymentRatio
            {
                Proposal = proposal1,
                Party = partyB,
                Amount = 400.00m,
                Status = ProposalStatus.Pending
            };

            var paymentRatio3 = new PaymentRatio
            {
                Proposal = counterProposal1,
                Party = partyA,
                Amount = 500.00m,
                Status = ProposalStatus.Accepted,
                StatusedBy = user1.UserId,
                User = user1
            };

            var paymentRatio4 = new PaymentRatio
            {
                Proposal = counterProposal1,
                Party = partyB,
                Amount = 500.00m,
                Status = ProposalStatus.Pending
            };

            var paymentRatio5 = new PaymentRatio
            {
                Proposal = proposal2,
                Party = partyB,
                Amount = 1500.00m,
                Status = ProposalStatus.Rejected,
                StatusedBy = user2.UserId,
                User = user2
            };

            var paymentRatio6 = new PaymentRatio
            {
                Proposal = proposal2,
                Party = partyC,
                Amount = 500.00m,
                Status = ProposalStatus.Pending
            };

            // Add Data to Context.
            _context.Parties.AddRange(partyA, partyB, partyC);
            _context.Users.AddRange(user1, user2, user3);
            _context.Items.AddRange(item1, item2, item3);
            _context.Proposals.AddRange(proposal1, proposal2, counterProposal1);
            _context.PaymentRatios.AddRange(paymentRatio1, paymentRatio2, paymentRatio3, paymentRatio4, paymentRatio5, paymentRatio6);

            _context.SaveChanges();
        }
    }
}
