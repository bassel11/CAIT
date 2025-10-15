using Microsoft.AspNetCore.Identity;

namespace Identity.Core.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public enum AuthenticationType
        {
            Database = 0,   // Local DB (fallback)
            OnPremAD = 1,   // Windows AD
            AzureAD = 2,    // Entra ID (Azure AD)
            B2BGuest = 3    // Entra External ID (Guest)
        }

        // ---------- User Identity ----------
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // مصدر المصادقة
        public AuthenticationType AuthType { get; set; } = AuthenticationType.Database;

        // ---------- Active Directory Fields ----------
        public string? AdDomain { get; set; }     // مثل CAIT.LOCAL أو cait.gov
        public string? AdAccount { get; set; }    // مثل ali أو ali@cait.gov

        // ---------- Azure AD / Entra Fields ----------
        public Guid? AzureTenantId { get; set; }  // Tenant GUID
        public Guid? AzureObjectId { get; set; }  // ObjectId للمستخدم في Entra

        // حقل عام للمعرّف الخارجي (للتوافق)
        public string? ExternalId { get; set; }

        // ---------- Security & Status ----------
        public bool MFAEnabled { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; } // حساب مؤقت (مثلاً للموردين)

        // ---------- Navigation Properties ----------
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        // ---------- Utility ----------
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
