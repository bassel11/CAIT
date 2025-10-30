using Identity.Application.Interfaces.Authorization;
using Identity.Core.Enums;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.Authorization
{
    public class PermissionChecker : IPermissionChecker
    {
        private readonly ApplicationDbContext _db;

        public PermissionChecker(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> HasPermissionAsync(Guid userId, string permissionName, Guid? committeeId = null)
        {
            // 1️⃣ المستخدم الـ SuperAdmin يتجاوز التفويض
            var userRoles = await _db.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role)
                .ToListAsync();

            if (userRoles.Any(r => r.Name == "SuperAdmin"))
                return true;

            // 2️⃣ جلب المعرف المطلوب للصلاحية
            var permission = await _db.Permissions.FirstOrDefaultAsync(p => p.Name == permissionName);
            if (permission == null) return false;

            // 3️⃣ تحقق من UserPermissionAssignment (منح مباشر)
            var userAllow = await _db.UserPermissionAssignments.AnyAsync(a =>
                a.UserId == userId &&
                a.PermissionId == permission.Id &&
                a.Allow == true &&
                (a.ScopeType == PermissionScopeType.Global ||
                 (a.ScopeType == PermissionScopeType.Committee && a.CommitteeId == committeeId)));

            if (userAllow) return true;

            var userDeny = await _db.UserPermissionAssignments.AnyAsync(a =>
                a.UserId == userId &&
                a.PermissionId == permission.Id &&
                a.Allow == false &&
                (a.ScopeType == PermissionScopeType.Global ||
                 (a.ScopeType == PermissionScopeType.Committee && a.CommitteeId == committeeId)));

            if (userDeny) return false;

            // 4️⃣ تحقق من RolePermission
            var roleIds = userRoles.Select(r => r.Id).ToList();

            var roleAllow = await _db.RolePermissions.AnyAsync(rp =>
                roleIds.Contains(rp.RoleId) &&
                rp.PermissionId == permission.Id &&
                rp.Allow == true &&
                (rp.ScopeType == PermissionScopeType.Global ||
                 (rp.ScopeType == PermissionScopeType.Committee && rp.CommitteeId == committeeId)));

            if (roleAllow) return true;

            return false;
        }
    }
}
