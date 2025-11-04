using Identity.Application.Interfaces.Authorization;
using Identity.Core.Entities;
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

        /// <summary>
        /// يتحقق من امتلاك المستخدم لصلاحية معينة على مورد معين (اختياري).
        /// </summary>
        public async Task<bool> HasPermissionAsync(
            Guid userId,
            string permissionName,
            Guid? resourceId = null)
        {
            // 1️⃣ تحقق من دور SuperAdmin (يتجاوز كل شيء)
            var userRoles = await _db.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role)
                .ToListAsync();

            if (userRoles.Any(r => r.Name == "SuperAdmin"))
                return true;

            // 2️⃣ احصل على الـ Permission المعني
            var permission = await _db.Permissions
                .FirstOrDefaultAsync(p => p.Name == permissionName && p.IsActive);

            if (permission == null)
                return false;

            // 🔹 اجلب المورد (إن وجد)
            Resource? resource = null;
            if (resourceId.HasValue)
            {
                resource = await _db.Resources.AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == resourceId.Value);
            }

            // ---------------------------------------------------
            // 3️⃣ تحقق من UserPermissionAssignments (صلاحيات فردية)
            // ---------------------------------------------------

            // Deny أولاً (الأولوية)
            bool userExplicitDeny = await _db.UserPermissionAssignments.AnyAsync(a =>
                a.UserId == userId &&
                a.PermissionId == permission.Id &&
                a.Allow == false &&
                (
                    a.ScopeType == PermissionScopeType.Global ||
                    (a.ScopeType == PermissionScopeType.ResourceType && a.Resource != null && a.Resource.ResourceType == permission.ResourceType) ||
                    (a.ScopeType == PermissionScopeType.ResourceInstance && a.ResourceId == resourceId)
                ));

            if (userExplicitDeny)
                return false;

            // Allow (إن وُجد)
            bool userExplicitAllow = await _db.UserPermissionAssignments.AnyAsync(a =>
                a.UserId == userId &&
                a.PermissionId == permission.Id &&
                a.Allow == true &&
                (
                    a.ScopeType == PermissionScopeType.Global ||
                    (a.ScopeType == PermissionScopeType.ResourceType && a.Resource != null && a.Resource.ResourceType == permission.ResourceType) ||
                    (a.ScopeType == PermissionScopeType.ResourceInstance && a.ResourceId == resourceId)
                ));

            if (userExplicitAllow)
                return true;

            // ---------------------------------------------------
            // 4️⃣ تحقق من صلاحيات الأدوار RolePermissions
            // ---------------------------------------------------
            var roleIds = userRoles.Select(r => r.Id).ToList();

            // Deny أولاً (الأولوية)
            bool roleExplicitDeny = await _db.RolePermissions.AnyAsync(rp =>
                roleIds.Contains(rp.RoleId) &&
                rp.PermissionId == permission.Id &&
                rp.Allow == false &&
                (
                    rp.ScopeType == PermissionScopeType.Global ||
                    (rp.ScopeType == PermissionScopeType.ResourceType && rp.Resource != null && rp.Resource.ResourceType == permission.ResourceType) ||
                    (rp.ScopeType == PermissionScopeType.ResourceInstance && rp.ResourceId == resourceId)
                ));

            if (roleExplicitDeny)
                return false;

            // Allow (إذا وُجد)
            bool roleAllow = await _db.RolePermissions.AnyAsync(rp =>
                roleIds.Contains(rp.RoleId) &&
                rp.PermissionId == permission.Id &&
                rp.Allow == true &&
                (
                    rp.ScopeType == PermissionScopeType.Global ||
                    (rp.ScopeType == PermissionScopeType.ResourceType && rp.Resource != null && rp.Resource.ResourceType == permission.ResourceType) ||
                    (rp.ScopeType == PermissionScopeType.ResourceInstance && rp.ResourceId == resourceId)
                ));

            if (roleAllow)
                return true;

            // ---------------------------------------------------
            // 5️⃣ تحقق من Parent Resource (مثل لجنة تابعة)
            // ---------------------------------------------------
            if (resource != null && resource.ParentReferenceId.HasValue)
            {
                var parent = await _db.Resources.AsNoTracking()
                    .FirstOrDefaultAsync(r =>
                        r.ReferenceId == resource.ParentReferenceId &&
                        r.ResourceType == resource.ParentResourceType);

                if (parent != null)
                {
                    // أعِد التحقق على مستوى parent
                    return await HasPermissionAsync(userId, permissionName, parent.Id);
                }
            }

            // ---------------------------------------------------
            // 6️⃣ الافتراضي: لا يوجد إذن
            // ---------------------------------------------------
            return false;
        }
    }
}
