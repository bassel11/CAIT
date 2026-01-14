using Identity.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs
{
    public class LoginDto : IValidatableObject
    {
        public string? Username { get; set; }
        [EmailAddress]
        public string? Email { get; set; }

        public string? Password { get; set; }

        [Required]
        public ApplicationUser.AuthenticationType AuthType { get; set; } = ApplicationUser.AuthenticationType.Database;

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (AuthType == ApplicationUser.AuthenticationType.Database)
            {
                if (string.IsNullOrWhiteSpace(Password))
                    yield return new ValidationResult("Password is required for Database login.", new[] { nameof(Password) });

                if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(Username))
                    yield return new ValidationResult("Either Email or Username must be provided.", new[] { nameof(Email), nameof(Username) });
            }

            if (AuthType == ApplicationUser.AuthenticationType.OnPremAD)
            {
                if (string.IsNullOrWhiteSpace(Password))
                    yield return new ValidationResult("Password is required for AD login.", new[] { nameof(Password) });
            }
        }
    }
}
