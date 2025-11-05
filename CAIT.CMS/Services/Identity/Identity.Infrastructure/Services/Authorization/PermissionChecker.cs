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
            // مفتاح التخزين المؤقت
            string cacheKey = $"user_permissions_{userId}";

            // محاولة استرجاع الصلاحيات من الكاش أولاً
            if (!_cache.TryGetValue(cacheKey, out HashSet<string> userPermissions))
            {
                // ✅ Eager Loading لجميع البيانات الضرورية دفعة واحدة
                var rolesWithPermissions = await _context.UserRoles
                    .Where(ur => ur.UserId == userId)
                    .Include(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                    .AsNoTracking()
                    .ToListAsync();

                // تجميع كل الصلاحيات في HashSet لتسريع البحث
                userPermissions = rolesWithPermissions
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Where(rp => resourceId == null || rp.ResourceId == resourceId) // دعم ResourceId إن وجد
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToHashSet();

                // تخزين في الكاش لمدة 10 دقائق
                _cache.Set(cacheKey, userPermissions, TimeSpan.FromMinutes(10));
            }

            return userPermissions.Contains(permissionName);
        }

        // اختياري: طريقة لتحديث الكاش عند تعديل صلاحيات المستخدم
        public void InvalidateCache(Guid userId)
        {
            string cacheKey = $"user_permissions_{userId}";
            _cache.Remove(cacheKey);
        }
    }

}
