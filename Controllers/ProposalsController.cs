using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NegotiationSystemAPI.Data;
using NegotiationSystemAPI.DTOs;
using NegotiationSystemAPI.Enums;
using NegotiationSystemAPI.Helpers;
using NegotiationSystemAPI.Models;

namespace NegotiationSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProposalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProposalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{proposalId}")]
        public async Task<ActionResult> GetProposalById(int proposalId)
        {
            var proposal = await _context.Proposals
                .Include(p => p.User.Party)
                .FirstOrDefaultAsync(p => p.ProposalId == proposalId);

            if (proposal is null)
            {
                return NotFound("Proposal not found.");
            }

            var result = new ProposalDto
            {
                ProposalId = proposal.ProposalId,
                CreationDate = proposal.CreationDate,
                CreatorParty = proposal.User.Party.Name,
                Message = proposal.Message,
                IsCounterProposal = proposal.IsCounterProposal
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> CreateProposal([FromBody] CreateProposalDto createProposalDto)
        {
            // Get the current user and their party.
            var userId = HttpContext.User.Identity?.Name;

            var user = await _context.Users
                .Include(u => u.Party)
                .FirstOrDefaultAsync(u => u.UserName == userId);

            if (user is null)
            {
                return Unauthorized("User is not authenticated.");
            }

            // Fetch the shared item.
            var item = await _context.Items
                .Include(i => i.Parties)
                .FirstOrDefaultAsync(i => i.ItemId == createProposalDto.ItemId);

            if (item is null)
            {
                return NotFound("Item not found.");
            }

            // Ensure that the item is shared (associated with more than one party).
            if (item.Parties.Count <= 1)
            {
                return BadRequest("Item is not shared among multiple parties.");
            }

            // Ensure the item is associated with the user's party.
            if (!item.Parties.Any(party => party.PartyId == user.Party.PartyId))
            {
                return BadRequest("Item is not associated with the user's party.");
            }

            // Check if there are existing proposals for the item.
            var existingProposal = await _context.Proposals
                .FirstOrDefaultAsync(p => p.ItemId == createProposalDto.ItemId);

            if (existingProposal != null)
            {
                return BadRequest("A proposal already exists. You must submit a counter-proposal.");
            }

            // Validate payments.
            ValidationResult paymentsValidationMessage = HelperMethods.ValidatePayments(
                    createProposalDto.Payments, 
                    item, createProposalDto.
                    IsPercentageBased
                );

            if (paymentsValidationMessage.Status != ValidationStatus.Valid)
            {
                return BadRequest(paymentsValidationMessage.Message);
            }

            // Create the proposal.
            var proposal = new Proposal
            {
                ItemId = createProposalDto.ItemId,
                CreationDate = DateTime.UtcNow,
                CreatedBy = user.UserId,
                Message = createProposalDto.Message?? string.Empty,
                IsCounterProposal = false
            };

            // Calculate and add the payment ratios to the proposal.
            HelperMethods.CreatePaymentRatios(proposal, createProposalDto, item);

            await _context.Proposals.AddAsync(proposal);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProposalById), new { proposalId = proposal.ProposalId }, proposal);
        }

        [HttpPost("{proposalId}/counter")]
        public async Task<ActionResult> CreateCounterProposal(int proposalId, [FromBody] CreateProposalDto createProposalDto)
        {
            // Get the current user and their party.
            var userId = HttpContext.User.Identity?.Name;

            var user = await _context.Users
                .Include(u => u.Party)
                .FirstOrDefaultAsync(u => u.UserName == userId);

            if (user is null)
            {
                return Unauthorized("User is not authenticated.");
            }

            // Fetch the original proposal.
            var originalProposal = await _context.Proposals
                .Include(p => p.User)
                .Include(p => p.Item.Parties)
                .FirstOrDefaultAsync(p => p.ProposalId == proposalId);

            if (originalProposal is null)
            {
                return NotFound("Original proposal not found.");
            }

            // Ensure the original proposal shared item exists.
            var item = originalProposal.Item;
            if (item is null)
            {
                return NotFound("Associated item not found.");
            }

            // Ensure the item is shared (associated with more than one party).
            if (item.Parties.Count <= 1)
            {
                return BadRequest("Item is not shared among multiple parties.");
            }

            // Ensure the item is associated with the user's party.
            if (!item.Parties.Any(party => party.PartyId == user.Party.PartyId))
            {
                return BadRequest("Item is not associated with the user's party.");
            }

            // Ensure the counter-proposal is not for the user's own proposal.
            if (originalProposal.CreatedBy == user.UserId)
            {
                return BadRequest("You cannot submit a counter-proposal to your own proposal.");
            }

            // Ensure a mandatory message/comment is provided.
            if (string.IsNullOrWhiteSpace(createProposalDto.Message))
            {
                return BadRequest("A message/comment is required for counter-proposals.");
            }

            // Validate payments.
            ValidationResult paymentsValidationMessage = HelperMethods.ValidatePayments(
                createProposalDto.Payments,
                item,
                createProposalDto.IsPercentageBased
            );

            if (paymentsValidationMessage.Status != ValidationStatus.Valid)
            {
                return BadRequest(paymentsValidationMessage.Message);
            }

            // Create the counter-proposal.
            var counterProposal = new Proposal
            {
                ItemId = originalProposal.ItemId,
                CreationDate = DateTime.UtcNow,
                CreatedBy = user.UserId,
                Message = createProposalDto.Message,
                IsCounterProposal = true,
                InitialProposalId = originalProposal.ProposalId // Link to the original proposal.
            };

            // Calculate and add the payment ratios to the counter-proposal.
            HelperMethods.CreatePaymentRatios(counterProposal, createProposalDto, item);

            await _context.Proposals.AddAsync(counterProposal);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProposalById), new { proposalId = counterProposal.ProposalId }, counterProposal);
        }

        [HttpPost("{proposalId}/accept")]
        public async Task<ActionResult> AcceptProposal(int proposalId)
        {
            var userId = HttpContext.User.Identity?.Name;

            var user = await _context.Users
                .Include(u => u.Party)
                .FirstOrDefaultAsync(u => u.UserName == userId);

            if (user is null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var proposal = await _context.Proposals
                .Include(p => p.PaymentRatios)
                .FirstOrDefaultAsync(p => p.ProposalId == proposalId);

            if (proposal is null)
            {
                return NotFound("Proposal not found.");
            }

            // Can't accept your own proposal.
            if (proposal.CreatedBy == user.UserId)
            {
                return BadRequest("You cannot accept your own proposal.");
            }

            // Ensure the user's party is part of the proposed payments.
            PaymentRatio? payment = proposal.PaymentRatios
                .Where(p => p.PartyId == user.PartyId)
                .FirstOrDefault();

            if (payment is null)
            {
                return BadRequest("User's party is not associated with any payment.");
            }

            payment.Status = ProposalStatus.Accepted;
            payment.StatusedBy = user.UserId;

            await _context.SaveChangesAsync();
            return Ok("Proposal accepted successfully.");
        }

        [HttpPost("{proposalId}/reject-and-counter")]
        public async Task<ActionResult> RejectAndCreateCounterProposal(int proposalId, [FromBody] CreateProposalDto createProposalDto)
        {
            var userId = HttpContext.User.Identity?.Name;

            var user = await _context.Users
                .Include(u => u.Party)
                .FirstOrDefaultAsync(u => u.UserName == userId);

            if (user is null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var proposal = await _context.Proposals
                .Include(p => p.PaymentRatios)
                .FirstOrDefaultAsync(p => p.ProposalId == proposalId);

            if (proposal is null)
            {
                return NotFound("Proposal not found.");
            }

            // Can't reject your own proposal.
            if (proposal.CreatedBy == user.UserId)
            {
                return BadRequest("You cannot reject your own proposal.");
            }

            // Ensure the user's party is part of the proposed payments.
            PaymentRatio? payment = proposal.PaymentRatios
                .Where(p => p.PartyId == user.PartyId)
                .FirstOrDefault();

            if (payment is null)
            {
                return BadRequest("User's party is not associated with any payment.");
            }

            payment.Status = ProposalStatus.Rejected;
            payment.StatusedBy = user.UserId;

            await _context.SaveChangesAsync();

            // Create the counter proposal.
            return await CreateCounterProposal(proposalId, createProposalDto);
        }

    }
}
