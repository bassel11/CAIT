using Identity.Core.Enums;
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
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {MiddleName} {FirstName}".Trim();
        public UserType UserType { get; set; } = UserType.InternalEmployee;
        public AuthenticationType AuthType { get; set; } = AuthenticationType.Database;

        // ---------- Active Directory Fields ----------
        public string? AdDomain { get; set; }     // مثل CAIT.LOCAL أو cait.gov
        public string? AdAccount { get; set; }    // مثل ali أو ali@cait.gov

        // ---------- Azure AD / Entra Fields ----------
        public Guid? AzureTenantId { get; set; }  // Tenant GUID
        public Guid? AzureObjectId { get; set; }  // ObjectId للمستخدم في Entra

        // حقل عام للمعرّف الخارجي (للتوافق)
        public string? ExternalId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? ExpirationDate { get; set; } // تاريخ انتهاء صلاحية الحساب
                                                      // مصدر المصادقة
                                                      // ---------- Security & Status ----------
        public bool MFAEnabled { get; set; } = false;

        public MFAMethod MFAMethod { get; set; } = MFAMethod.None;
        public string? AuthenticatorKey { get; set; } // المفتاح الخاص بـ TOTP

        //public string? MFASecret { get; set; }  // لتخزين مفتاح TOTP
        public string? MFACode { get; set; }          // الكود المؤقت

        public string? MFACodeHash { get; set; }  //  الكود المؤقت المشفر
        public DateTime? MFACodeExpiry { get; set; }  // انتهاء صلاحية الكود

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; } // حساب مؤقت (مثلاً للموردين)

        // ---------- Navigation Properties ----------
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        #region Pre for new Privilage method
        public virtual ICollection<UserRolePermReso> UserRolePermResos { get; set; } = new List<UserRolePermReso>();
        public PrivilageType PrivilageType { get; set; } = PrivilageType.None;
        #endregion

    }
}
