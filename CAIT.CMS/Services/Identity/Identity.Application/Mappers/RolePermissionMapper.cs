using Identity.Application.DTOs.RolePermissions;
using Identity.Core.Entities;
using System.Linq.Expressions;

namespace Identity.Application.Mappers
{
    public static class RolePermissionMapper
    {
        public static readonly Expression<Func<RolePermission, RolePermissionDetailsDto>> ToDtoExpr = rp => new RolePermissionDetailsDto
        {
            RoleId = rp.RoleId,
            PermissionId = rp.PermissionId,
            PermissionName = rp.Permission.Name,
            Description = rp.Permission.Description,
            PermissionResourceType = rp.Permission.ResourceType,
            PermissionResourceTypeName = rp.Permission.ResourceType.ToString(),
            Action = rp.Permission.Action,
            ActionName = rp.Permission.Action.ToString(),
            PermissionScopeType = rp.ScopeType,
            PermissionScopeTypeName = rp.ScopeType.ToString(),
            IsGlobal = rp.Permission.IsGlobal,
            IsActive = rp.Permission.IsActive,
            Allow = rp.Allow,
            CreatedAt = rp.CreatedAt,

            // 🟢 Resource Info
            ResourceId = rp.ResourceId,
            ResourceReferenceId = rp.Resource != null ? rp.Resource.ReferenceId : null,
            Res_Type = rp.Resource != null ? rp.Resource.ResourceType : null,
            Res_TypeName = rp.Resource != null ? rp.Resource.ResourceType.ToString() : null,
            ResourceDisplayName = rp.Resource != null ? rp.Resource.DisplayName : null,
            ResourceParentType = rp.Resource != null ? rp.Resource.ParentResourceType : null,
            ResourceParentTypeName = rp.Resource != null ? rp.Resource.ParentResourceType.ToString() : null,
            ResourceParentReferenceId = rp.Resource != null ? rp.Resource.ParentReferenceId : null
        };

        public static RolePermissionDetailsDto ToDto(RolePermission rp)
        {
            if (rp == null) return null!;
            return ToDtoExpr.Compile().Invoke(rp);
        }
    }
}
