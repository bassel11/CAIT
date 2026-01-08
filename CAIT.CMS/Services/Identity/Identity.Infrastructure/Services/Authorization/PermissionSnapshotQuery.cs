using Identity.Application.DTOs.Snapshot;
using Identity.Application.Interfaces.Authorization;
using Identity.Core.Enums;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.Authorization
{
    public sealed class PermissionSnapshotQuery : IPermissionSnapshotQuery
    {
        private readonly ApplicationDbContext _context;

        public PermissionSnapshotQuery(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SnapshotData> GetSnapshotsAsync(Guid userId)
        {
            // 1. جلب معلومات المستخدم الأساسية أولاً لتحديد النوع
            var user = await _context.Users
                .AsNoTracking()
                .Select(u => new { u.Id, u.IsActive, u.PrivilageType })
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

            // إذا لم يوجد مستخدم، نعيد النوع None وقائمة فارغة
            if (user == null)
            {
                return new SnapshotData
                {
                    UserPrivilageType = PrivilageType.None,
                    Permissions = Array.Empty<FullUserPermission>()
                };
            }

            // 2. جلب الصلاحيات بناءً على النوع
            IReadOnlyList<FullUserPermission> permissions = user.PrivilageType switch
            {
                PrivilageType.None => Array.Empty<FullUserPermission>(),

                PrivilageType.PredifinedRoles => await GetPredefinedRoleSnapshots(userId),

                PrivilageType.CustomRolesAndPermission => await GetCustomPermissionSnapshots(userId),

                _ => Array.Empty<FullUserPermission>()
            };

            // 3. إرجاع النتيجة المجمعة
            return new SnapshotData
            {
                UserPrivilageType = user.PrivilageType,
                Permissions = permissions
            };
        }

        private async Task<IReadOnlyList<FullUserPermission>> GetPredefinedRoleSnapshots(Guid userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission)
                .AsNoTracking()
                .Where(p => p.IsActive) // تحقق إضافي أن الصلاحية فعالة
                .Distinct()
                .Select(p => new FullUserPermission
                {
                    Name = p.Name,
                    Description = p.Description,
                    IsActive = p.IsActive,
                    Scope = ScopeType.Global,
                    ScopeName = "Global",
                    Allow = true
                })
                .ToListAsync();
        }

        private async Task<IReadOnlyList<FullUserPermission>> GetCustomPermissionSnapshots(Guid userId)
        {
            return await _context.UserRolePermResos
                .Where(x => x.UserId == userId)
                .AsNoTracking()
                // يمكن إضافة شرط IsActive للصلاحية هنا أيضاً
                .Where(x => x.Permission.IsActive)
                .Select(r => new FullUserPermission
                {
                    Name = r.Permission.Name,
                    Description = r.Permission.Description,
                    IsActive = r.Permission.IsActive,
                    Scope = r.Scope,
                    ScopeName = r.Scope.ToString(),
                    ResourceType = r.ResourceId.HasValue ? r.ResourceType : null,
                    ResourceTypeName = r.ResourceId.HasValue ? r.ResourceType.ToString() : null,
                    ResourceId = r.ResourceId,
                    ParentResourceType = r.ParentResourceId.HasValue ? r.ParentResourceType : null,
                    ParentResourceTypeName = r.ParentResourceId.HasValue ? r.ParentResourceType.ToString() : null,
                    ParentResourceId = r.ParentResourceId,
                    Allow = r.Allow
                })
                .OrderByDescending(x => x.Allow)
                .ToListAsync();
        }
    }
}