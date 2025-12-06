using Identity.Application.Interfaces.Authorization;
using Identity.Core.Entities;
using Identity.Core.Enums;
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

        public async Task<bool> HasPermissionAsync(Guid userId, string permissionName, Guid? resourceId = null, Guid? parentResourceId = null)
        {
            // 1️⃣ جلب المستخدم لمعرفة PrivilageType
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || !user.IsActive)
                return false;

            switch (user.PrivilageType)
            {
                // 2️⃣ المستخدم لا يملك أي صلاحيات
                case PrivilageType.None:
                    return false;

                // 3️⃣ صلاحيات قائمة على الأدوار القياسية (PredifinedRoles)
                case PrivilageType.PredifinedRoles:
                    return await CheckPredefinedRolePermissions(userId, permissionName);

                // 4️⃣ صلاحيات مخصصة (CustomRolesAndPermission)
                case PrivilageType.CustomRolesAndPermission:
                    return await CheckCustomPermissions(userId, permissionName, resourceId, parentResourceId);

                default:
                    return false;
            }
        }

        // ------------------------------------------
        // 🔹 تحقق من صلاحيات PredifinedRoles
        // ------------------------------------------
        private async Task<bool> CheckPredefinedRolePermissions(Guid userId, string permissionName)
        {
            string cacheKey = $"user_predef_perms_{userId}";

            if (!_cache.TryGetValue(cacheKey, out HashSet<string>? userPermissions))
            {
                var rolesWithPermissions = await _context.UserRoles
                    .Where(ur => ur.UserId == userId)
                    .Include(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                    .AsNoTracking()
                    .ToListAsync();

                userPermissions = rolesWithPermissions
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToHashSet();

                _cache.Set(cacheKey, userPermissions, TimeSpan.FromMinutes(5));
            }

            return userPermissions.Contains(permissionName);
        }

        // ------------------------------------------
        // 🔹 تحقق من صلاحيات CustomRolesAndPermission
        // ------------------------------------------
        private async Task<bool> CheckCustomPermissions(Guid userId, string permissionName, Guid? resourceId, Guid? parentResourceId)
        {
            IQueryable<UserRolePermReso> query = _context.UserRolePermResos
                .Include(x => x.Permission)
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.Permission.Name == permissionName);

            // 🟦 الحالة (أ): بدون ResourceId / ParentResourceId
            if (resourceId == null && parentResourceId == null)
            {
                return await query.AnyAsync();
            }

            // 🟦 الحالة (ب): مع ResourceId فقط
            if (resourceId != null && parentResourceId == null)
            {
                return await query.AnyAsync(x => x.ResourceId == resourceId);
            }

            // 🟦 الحالة (ج): مع ResourceId + ParentResourceId
            if (resourceId != null && parentResourceId != null)
            {
                return await query.AnyAsync(x =>
                    x.ResourceId == resourceId &&
                    x.ParentResourceId == parentResourceId);
            }

            return false;
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
