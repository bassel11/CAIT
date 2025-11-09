using Identity.Application.Interfaces.Authorization;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.Authorization
{
    public class PermissionCacheInvalidator : IPermissionCacheInvalidator
    {
        private readonly ApplicationDbContext _context;
        private readonly IPermissionChecker _permissionChecker;

        public PermissionCacheInvalidator(ApplicationDbContext context, IPermissionChecker permissionChecker)
        {
            _context = context;
            _permissionChecker = permissionChecker;
        }

        public async Task InvalidateUserPermissionsByRoleAsync(Guid roleId)
        {
            var userIds = await _context.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => ur.UserId)
                .Distinct()
                .ToListAsync();

            foreach (var userId in userIds)
                _permissionChecker.InvalidateCache(userId);
        }

        public async Task InvalidateUserPermissionsByUserAsync(Guid userId)
        {
            _permissionChecker.InvalidateCache(userId);
            await Task.CompletedTask;
        }
    }
}
