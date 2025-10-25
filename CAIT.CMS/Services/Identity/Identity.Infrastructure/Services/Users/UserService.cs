using Identity.Application.Common;
using Identity.Application.DTOs.Users;
using Identity.Application.Interfaces.Users;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Services.Users
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
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
                query = query.Where(u =>
                    u.FirstName.Contains(filter.Search) ||
                    u.LastName.Contains(filter.Search) ||
                    u.Email!.Contains(filter.Search));

            if (filter.IsActive.HasValue)
                query = query.Where(u => u.IsActive == filter.IsActive);

            if (filter.AuthType.HasValue)
                query = query.Where(u => u.AuthType == filter.AuthType);

            if (filter.MFAEnabled.HasValue)
                query = query.Where(u => u.MFAEnabled == filter.MFAEnabled);

            // 🕒 التصفية بالتواريخ
            if (filter.CreatedAfter.HasValue)
                query = query.Where(u => u.CreatedAt >= filter.CreatedAfter.Value);
            if (filter.CreatedBefore.HasValue)
                query = query.Where(u => u.CreatedAt <= filter.CreatedBefore.Value);

            var total = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(u => new UserDto
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
                    Roles = u.UserRoles.Select(r => r.Role.Name!).ToList()
                })
                .ToListAsync();

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
    }
}
