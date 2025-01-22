using NegotiationSystemAPI.Enums;

namespace NegotiationSystemAPI.Helpers
{
    public class ValidationResult
    {
        public ValidationStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;

        public static ValidationResult Success(string message = "Validation succeeded.")
        {
            return new ValidationResult
            {
                Status = ValidationStatus.Valid,
                Message = message
            };
        }

        public static ValidationResult Failure(string message)
        {
            return new ValidationResult
            {
                Status = ValidationStatus.Invalid,
                Message = message
            };
        }

        public static ValidationResult Warning(string message)
        {
            return new ValidationResult
            {
                Status = ValidationStatus.Warning,
                Message = message
            };
        }
    }
}
