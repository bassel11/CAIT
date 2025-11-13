using Identity.Application.DTOs.RolePermissions;
using Identity.Core.Entities;
using System.Linq.Expressions;

namespace Identity.Application.Mappers
{
    public static class RolePermissionMapper
    {
        public static readonly Expression<Func<RolePermission, PermsDetailsDto>> ToDtoExpr = rp => new PermsDetailsDto
        {
            RoleId = rp.RoleId,
            PermissionId = rp.PermissionId,
            PermissionName = rp.Permission.Name,
            Description = rp.Permission.Description,
            PermissionResourceType = rp.Permission.ResourceType,
            PermissionResourceTypeName = rp.Permission.ResourceType.ToString(),
            Action = rp.Permission.Action,
            ActionName = rp.Permission.Action.ToString(),
            IsGlobal = rp.Permission.IsGlobal,
            IsActive = rp.Permission.IsActive

        };

        public static PermsDetailsDto ToDto(RolePermission rp)
        {
            if (rp == null) return null!;
            return ToDtoExpr.Compile().Invoke(rp);
        }
    }
}
