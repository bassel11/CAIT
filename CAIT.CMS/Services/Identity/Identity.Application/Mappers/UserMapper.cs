using Identity.Application.DTOs.Users;
using Identity.Core.Entities;
using System.Linq.Expressions;

namespace Identity.Application.Mappers
{
    public static class UserMapper
    {
        // Expression Mapper (يُترجم مباشرة إلى SQL داخل EF Core)
        public static readonly Expression<Func<ApplicationUser, UserDto>> ToDtoExpr = u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
            NormalizedUserName = u.NormalizedUserName,
            Email = u.Email,
            NormalizedEmail = u.NormalizedEmail,
            EmailConfirmed = u.EmailConfirmed,
            PhoneNumber = u.PhoneNumber,
            PhoneNumberConfirmed = u.PhoneNumberConfirmed,
            TwoFactorEnabled = u.TwoFactorEnabled,
            LockoutEnd = u.LockoutEnd,
            LockoutEnabled = u.LockoutEnabled,
            AccessFailedCount = u.AccessFailedCount,
            FirstName = u.FirstName,
            LastName = u.LastName,
            ExpirationDate = u.ExpirationDate,
            AuthType = u.AuthType,
            AdDomain = u.AdDomain,
            AdAccount = u.AdAccount,
            AzureTenantId = u.AzureTenantId,
            AzureObjectId = u.AzureObjectId,
            ExternalId = u.ExternalId,
            MFAEnabled = u.MFAEnabled,
            MFAMethod = u.MFAMethod,
            MFACodeExpiry = u.MFACodeExpiry,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt,
            ExpiresAt = u.ExpiresAt,
            PrivilageType = u.PrivilageType,
            Roles = u.UserRoles.Select(r => r.Role.Name!).ToList()
        };

        // Helper لتحويل كيان موجود بالفعل إلى DTO في الذاكرة
        public static UserDto ToDto(ApplicationUser? u)
        {
            if (u == null) return null!;
            return ToDtoExpr.Compile().Invoke(u);
        }
    }
}
