using Identity.Application.Common;
using Identity.Application.DTOs.Users;
using Identity.Application.Interfaces.Users;
using Identity.Application.Mappers;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Services.Users
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }
        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _context.Users
           .Include(u => u.UserRoles)
           .ThenInclude(ur => ur.Role)
           .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                NormalizedUserName = user.NormalizedUserName,
                Email = user.Email,
                NormalizedEmail = user.NormalizedEmail,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnd = user.LockoutEnd,
                LockoutEnabled = user.LockoutEnabled,
                AccessFailedCount = user.AccessFailedCount,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ExpirationDate = user.ExpirationDate,
                AuthType = user.AuthType,
                AdDomain = user.AdDomain,
                AdAccount = user.AdAccount,
                AzureTenantId = user.AzureTenantId,
                AzureObjectId = user.AzureObjectId,
                ExternalId = user.ExternalId,
                MFAEnabled = user.MFAEnabled,
                MFAMethod = user.MFAMethod,
                MFACodeExpiry = user.MFACodeExpiry,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                ExpiresAt = user.ExpiresAt,
                Roles = user.UserRoles.Select(r => r.Role.Name!).ToList()
            };
        }

        public async Task<PagedResult<UserDto>> GetUsersAsync(UserFilterDto filter)
        {
            var query = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .AsQueryable();

            // 🧭 الفلاتر
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var s = filter.Search.Trim();
                query = query.Where(u =>
                    u.FirstName.Contains(s) ||
                    u.LastName.Contains(s) ||
                    u.Email!.Contains(s));
            }

            if (filter.IsActive.HasValue)
                query = query.Where(u => u.IsActive == filter.IsActive);

            if (filter.AuthType.HasValue)
                query = query.Where(u => u.AuthType == filter.AuthType);

            if (filter.MFAEnabled.HasValue)
                query = query.Where(u => u.MFAEnabled == filter.MFAEnabled);

            if (filter.CreatedAfter.HasValue)
                query = query.Where(u => u.CreatedAt >= filter.CreatedAfter.Value);
            if (filter.CreatedBefore.HasValue)
                query = query.Where(u => u.CreatedAt <= filter.CreatedBefore.Value);

            // Count before paging
            var total = await query.CountAsync();

            // Sort map
            var sortMap = new Dictionary<string, Expression<Func<ApplicationUser, object>>>
            {
                ["username"] = u => u.UserName,
                ["firstname"] = u => u.FirstName,
                ["lastname"] = u => u.LastName,
                ["email"] = u => u.Email,
                ["createdat"] = u => u.CreatedAt
            };

            // Apply sorting & paging
            query = query.ApplySorting(filter.SortBy ?? "createdat", filter.SortDir, sortMap)
                         .ApplyPaging(filter);

            // Projection using UserMapper
            var users = await query.Select(UserMapper.ToDtoExpr).ToListAsync();

            return new PagedResult<UserDto>(users, total, filter.Page, filter.PageSize);
        }


        public async Task<(bool Success, string? Error)> UpdateAsync(Guid id, UserUpdateDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return (false, "User not found");

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
            user.MFAEnabled = dto.MFAEnabled ?? user.MFAEnabled;
            user.IsActive = dto.IsActive ?? user.IsActive;
            user.ExpirationDate = dto.ExpirationDate ?? user.ExpirationDate;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("User {UserId} updated at {Time}", id, DateTime.UtcNow);

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> SoftDeleteAsync(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return (false, "User not found");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} soft-deleted at {Time}", id, DateTime.UtcNow);
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeactivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "User not found");

            if (!user.IsActive)
                return (false, "User is already deactivated");

            // تعطيل المستخدم
            user.IsActive = false;
            await _userManager.UpdateAsync(user);

            // تسجيل العملية في الـ Audit
            //await _auditService.LogAsync(new AuditLog
            //{
            //    UserId = User.Identity.Name,
            //    Action = "DeactivateUser",
            //    TargetUserId = user.Id,
            //    Timestamp = DateTime.UtcNow,
            //    Description = $"User {user.UserName} has been deactivated"
            //});

            return (true, null);

        }

    }
}
