using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NegotiationSystemAPI.Data;
using NegotiationSystemAPI.DTOs;
using NegotiationSystemAPI.Helpers;

namespace NegotiationSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("party")]
        public async Task<ActionResult> GetPartyItems()
        {
            // Get the current user and their party.
            var userName = HttpContext.User.Identity?.Name;

            var user = await _context.Users
                .Include(u => u.Party)
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user is null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var party = user!.Party;

            if (party is null)
            {
                return NotFound("Party not found for the current user.");
            }

            // Filter by the user's party.
            List<ItemDto> items = await _context.Items
                .Where(i => i.Parties.Any(p => p.PartyId == party.PartyId))
                .Select(i => new ItemDto
                {
                    ItemId = i.ItemId,
                    Name = i.Name,
                    CreationDate = i.CreationDate,
                    Price = i.Price,
                    IsShared = i.Parties.Count() > 1
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllItems([FromQuery] ItemQueryParametersDto queryParams)
        {
            var itemsQuery = _context.Items.AsQueryable();

            // Apply filters based on query parameters.
            if (!string.IsNullOrWhiteSpace(queryParams.Name))
            {
                itemsQuery = itemsQuery.Where(i => i.Name.Contains(queryParams.Name));
            }

            if (queryParams.StartDate.HasValue)
            {
                itemsQuery = itemsQuery.Where(i => i.CreationDate >= queryParams.StartDate.Value);
            }

            if (queryParams.EndDate.HasValue)
            {
                itemsQuery = itemsQuery.Where(i => i.CreationDate <= queryParams.EndDate.Value);
            }

            if (queryParams.IsShared.HasValue)
            {
                // Shared if the item has more than one associated party.
                itemsQuery = queryParams.IsShared.Value
                    ? itemsQuery.Where(i => i.Parties.Count > 1)
                    : itemsQuery.Where(i => i.Parties.Count == 1);
            }

            // Get the item property we're ordering by.
            var sortProperty = HelperMethods.GetSortProperty(queryParams.SortColumn);

            if (queryParams.SortOrder?.ToLower() == "desc")
            {
                itemsQuery = itemsQuery.OrderByDescending(sortProperty);
            }
            else
            {
                itemsQuery = itemsQuery.OrderBy(sortProperty);
            }

            var items = await itemsQuery.Select(i => new ItemDto
            {
                ItemId = i.ItemId,
                Name = i.Name,
                CreationDate = i.CreationDate,
                Price = i.Price,
                IsShared = i.Parties.Count() > 1
            }).ToListAsync();

            return Ok(items);
        }

        [HttpGet("{itemId}/negotiations")]
        public async Task<ActionResult> GetNegotiationDetails(int itemId)
        {
            // Get the current user and their party.
            var userName = HttpContext.User.Identity?.Name;
            
            var user = await _context.Users
                .Include(u => u.Party)
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user is null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var userPartyId = user!.PartyId;

            // Check if the item exists and is shared.
            var item = await _context.Items
                .Include(i => i.Parties)
                .FirstOrDefaultAsync(i => i.ItemId == itemId);

            if (item is null)
            {
                return NotFound("Item not found.");
            }

            if (item.Parties.Count <= 1)
            {
                return BadRequest("Item is not shared.");
            }

            // Get all proposals and counter-proposals for the item.
            var proposals = await _context.Proposals
                .Include(proposal => proposal.User.Party)
                .Include(proposal => proposal.PaymentRatios)
                .ThenInclude(payment => payment.Party)
                .Include(proposal => proposal.PaymentRatios)
                .ThenInclude(payment => payment.User.Party)
                .Where(proposal => proposal.ItemId == itemId)
                .OrderBy(proposal => proposal.CreationDate)
                .ToListAsync();

            List<ProposalDto> results = new List<ProposalDto>();

            foreach(var proposal in proposals)
            {
                ProposalDto proposalDto = new ProposalDto
                {
                    ProposalId = proposal.ProposalId,
                    CreationDate = proposal.CreationDate,
                    Message = proposal.Message,
                    CreatorName = HelperMethods.GetProposalCreatorName(proposal, userPartyId),
                    CreatorParty = proposal.User.Party.Name,
                    IsCounterProposal = proposal.IsCounterProposal,
                    InitialProposalId = proposal.InitialProposalId
                };

                proposalDto.Payments = proposal.PaymentRatios.Select(payment => new PaymentRatioDto
                {
                    PaymentRatioId = payment.PaymentRatioId,
                    PartyId = payment.PartyId,
                    PartyName = payment.Party.Name,
                    Amount = payment.Amount,
                    Status = payment.Status.ToString(),
                    ReviewerName = HelperMethods.GetPaymentReviewerName(payment, userPartyId),
                    ReviewerParty = HelperMethods.GetPaymentReviewerPartyName(payment)
                }).ToList();

                results.Add(proposalDto);
            }

            return Ok(results);
        }
    }
}
