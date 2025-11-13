using Identity.Core.Entities;
using Identity.Core.Enums;

namespace Identity.Application.DTOs.Users
{
    public class UserDto
    {
        public Guid Id { get; set; }

        // IdentityUser base
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        // ---------- User Identity ----------
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();
        public DateTime? ExpirationDate { get; set; }

        public ApplicationUser.AuthenticationType AuthType { get; set; }

        // ---------- Active Directory ----------
        public string? AdDomain { get; set; }
        public string? AdAccount { get; set; }

        // ---------- Azure AD ----------
        public Guid? AzureTenantId { get; set; }
        public Guid? AzureObjectId { get; set; }
        public string? ExternalId { get; set; }

        // ---------- Security ----------
        public bool MFAEnabled { get; set; }
        public MFAMethod MFAMethod { get; set; }
        public DateTime? MFACodeExpiry { get; set; }
        public bool IsActive { get; set; }
        public PrivilageType PrivilageType { get; set; }

        // ---------- Dates ----------
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        // ---------- Roles ----------
        public List<string> Roles { get; set; } = new();
    }
}
