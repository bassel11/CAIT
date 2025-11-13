using Identity.Application.DTOs.Pre.Custom;
using Identity.Core.Entities;
using System.Linq.Expressions;

namespace Identity.Application.Mappers
{
    public static class CustomPermsMapper
    {
        public static readonly Expression<Func<UserRolePermReso, CustomPermsDetailsDto>> ToDtoExpr = rp => new CustomPermsDetailsDto
        {
            UserId = rp.Id,
            UserName = rp.User.UserName!,
            RoleId = rp.RoleId,
            RoleName = rp.Role.Name!,
            PermissionId = rp.PermissionId,
            PermissionName = rp.Permission.Name,
            Scope = rp.Scope,
            ResourceType = rp.ResourceType,
            ResourceTypeName = rp.ResourceType.ToString(),
            ResourceId = rp.ResourceId,
            ParentResourceType = rp.ParentResourceType,
            ParentResourceTypeName = rp.ParentResourceType.ToString(),
            ParentResourceId = rp.ParentResourceId,
            Allow = rp.Allow
        };

        public static CustomPermsDetailsDto ToDto(UserRolePermReso rp)
        {
            if (rp == null) return null!;
            return ToDtoExpr.Compile().Invoke(rp);
        }
    }
}
