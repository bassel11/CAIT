using Identity.Application.Interfaces.Authorization;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Identity.Infrastructure.Services.Authorization
{
    public class PermissionChecker : IPermissionChecker
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public PermissionChecker(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<bool> HasPermissionAsync(Guid userId, string permissionName, Guid? resourceId = null)
        {
            string cacheKey = $"user_permissions_{userId}";

            if (!_cache.TryGetValue(cacheKey, out HashSet<string> userPermissions))
            {
                // تحميل الصلاحيات من قاعدة البيانات
                var rolesWithPermissions = await _context.UserRoles
                    .Where(ur => ur.UserId == userId)
                    .Include(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                    .AsNoTracking()
                    .ToListAsync();

                userPermissions = rolesWithPermissions
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Where(rp => resourceId == null || rp.ResourceId == resourceId)
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToHashSet();

                // تخزين في الكاش لمدة دقيقة واحدة
                _cache.Set(cacheKey, userPermissions, TimeSpan.FromMinutes(1));
            }

            return userPermissions.Contains(permissionName);
        }

        /// <summary>
        /// إبطال الكاش عند تعديل الصلاحيات
        /// </summary>
        public void InvalidateCache(Guid userId)
        {
            string cacheKey = $"user_permissions_{userId}";
            _cache.Remove(cacheKey);
        }
    }

}
