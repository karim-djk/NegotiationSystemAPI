using NegotiationSystemAPI.DTOs;
using NegotiationSystemAPI.Enums;
using NegotiationSystemAPI.Models;
using System.Linq.Expressions;

namespace NegotiationSystemAPI.Helpers
{
    public static class HelperMethods
    {
        public static Expression<Func<Item, object>> GetSortProperty(string? sortBy)
        {
            return sortBy?.ToLower() switch
            {
                "name" => item => item.Name,
                "date" => item => item.CreationDate,
                "price" => item => item.Price,
                _ => item => item.ItemId
            };
        }

        public static ValidationResult ValidatePayments(List<CreatePaymentRatioDto> payments, Item item, bool isPercentageBased)
        {
            // Check for duplicate parties.
            var duplicatePartyIds = payments
                .GroupBy(ratio => ratio.PartyId)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicatePartyIds.Any())
            {
                return ValidationResult.Failure($"Payments include duplicate parties.");
            }

            // check if each payment party is part of the item's associated parties.
            if (payments.Any(payment => !item.Parties.Any(p => p.PartyId == payment.PartyId)))
            {
                return ValidationResult.Failure("You can only create payment ratios for parties associated to that item.");
            }

            if (isPercentageBased)
            {
                if (payments.Any(p => !p.Percentage.HasValue))
                {
                    return ValidationResult.Failure("Invalid payment percentage found.");
                }

                if (payments.Sum(p => p.Percentage) != 100)
                {
                    return ValidationResult.Failure("Payment percentages do not add up to 100%.");
                }
            }
            else
            {
                if (payments.Any(p => !p.Amount.HasValue))
                {
                    return ValidationResult.Failure("Invalid payment amount found.");
                }

                if (payments.Sum(p => p.Amount) != item.Price)
                {
                    return ValidationResult.Failure("Payment amounts do not add up to item's price.");
                }
            }

            return ValidationResult.Success();
        }

        public static void CreatePaymentRatios(Proposal proposal, CreateProposalDto createProposalDto, Item item)
        {
            foreach (var payment in createProposalDto.Payments)
            {
                decimal amount = createProposalDto.IsPercentageBased
                    ? item.Price * (payment.Percentage!.Value / 100m)
                    : payment.Amount!.Value;

                PaymentRatio paymentRatio = new()
                {
                    PartyId = payment.PartyId,
                    Amount = amount,
                    Status = ProposalStatus.Pending
                };

                proposal.PaymentRatios.Add(paymentRatio);
            }
        }

        public static string GetProposalCreatorName(Proposal proposal, int userPartyId)
        {
            return proposal.User.PartyId == userPartyId
                        ? $"{proposal.User.FirstName} {proposal.User.LastName}"
                        : string.Empty;
        }

        public static string GetPaymentReviewerName(PaymentRatio payment, int userPartyId)
        {
            if (payment.User is null)
            {
                return string.Empty;
            }

            return payment.User.PartyId == userPartyId
                        ? $"{payment.User.FirstName} {payment.User.LastName}"
                        : string.Empty;
        }

        public static string GetPaymentReviewerPartyName(PaymentRatio payment)
        {
            if (payment.User is null)
            {
                return string.Empty;
            }

            return payment.User.Party.Name;
        }
    }
}
