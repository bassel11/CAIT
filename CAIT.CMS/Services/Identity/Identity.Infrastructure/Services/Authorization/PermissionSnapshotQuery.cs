using Identity.Application.DTOs.Snapshot;
using Identity.Application.Interfaces.Authorization;
using Identity.Core.Enums;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.Authorization
{
    /// <summary>
    /// Responsible for building Permission Snapshots (Read Model)
    /// No caching here – caching should be applied at higher layers (Redis)
    /// </summary>
    public sealed class PermissionSnapshotQuery : IPermissionSnapshotQuery
    {
        private readonly ApplicationDbContext _context;

        public PermissionSnapshotQuery(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all permission snapshots for a user based on PrivilageType
        /// </summary>
        public async Task<IReadOnlyList<FullUserPermission>> GetSnapshotsAsync(Guid userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Select(u => new { u.Id, u.IsActive, u.PrivilageType })
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

            if (user == null)
                return Array.Empty<FullUserPermission>();

            return user.PrivilageType switch
            {
                PrivilageType.None =>
                    Array.Empty<FullUserPermission>(),

                PrivilageType.PredifinedRoles =>
                    await GetPredefinedRoleSnapshots(userId),

                PrivilageType.CustomRolesAndPermission =>
                    await GetCustomPermissionSnapshots(userId),

                _ => Array.Empty<FullUserPermission>()
            };
        }

        // -------------------------------------------------
        // 🔹 Predefined Roles Permissions (Global scope)
        // -------------------------------------------------
        private async Task<IReadOnlyList<FullUserPermission>> GetPredefinedRoleSnapshots(Guid userId)
        {
            var permissions = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission)
                .AsNoTracking()
                .Distinct()
                .Select(p => new FullUserPermission
                {
                    Name = p.Name,
                    Description = p.Description,
                    IsActive = p.IsActive,

                    Scope = ScopeType.Global,
                    ScopeName = ScopeType.Global.ToString(),

                    ResourceType = null,
                    ResourceTypeName = null,
                    ResourceId = null,

                    ParentResourceType = null,
                    ParentResourceTypeName = null,
                    ParentResourceId = null,

                    Allow = true
                })
                .ToListAsync();

            return permissions;
        }

        // -------------------------------------------------
        // 🔹 Custom Roles & Permissions (Resource based)
        // -------------------------------------------------
        private async Task<IReadOnlyList<FullUserPermission>> GetCustomPermissionSnapshots(Guid userId)
        {
            var records = await _context.UserRolePermResos
                .Where(x => x.UserId == userId)
                .AsNoTracking()
                .Select(r => new FullUserPermission
                {
                    Name = r.Permission.Name,
                    Description = r.Permission.Description,
                    IsActive = r.Permission.IsActive,

                    Scope = r.Scope,
                    ScopeName = r.Scope.ToString(),

                    ResourceType = r.ResourceId.HasValue ? r.ResourceType : null,
                    ResourceTypeName = r.ResourceId.HasValue
                        ? r.ResourceType.ToString()
                        : null,
                    ResourceId = r.ResourceId,

                    ParentResourceType = r.ParentResourceId.HasValue ? r.ParentResourceType : null,
                    ParentResourceTypeName = r.ParentResourceId.HasValue
                        ? r.ParentResourceType.ToString()
                        : null,
                    ParentResourceId = r.ParentResourceId,

                    Allow = r.Allow
                })
                .OrderByDescending(x => x.Allow) // Allow > Deny
                .ToListAsync();

            return records;
        }
    }
}
