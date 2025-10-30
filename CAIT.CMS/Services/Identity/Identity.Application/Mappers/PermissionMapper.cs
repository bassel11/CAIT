using Identity.Application.DTOs.Permissions;
using Identity.Core.Entities;
using System.Linq.Expressions;

namespace Identity.Application.Mappers
{
    public static class PermissionMapper
    {
        public static readonly Expression<Func<Permission, PermissionDto>> ToDtoExpr = p => new PermissionDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Resource = p.ResourceType.ToString(),
            Action = p.Action.ToString(),
            IsGlobal = p.IsGlobal,
            IsActive = p.IsActive
        };

        // helper if you need to map an already-loaded entity
        public static PermissionDto ToDto(Permission? p)
        {
            if (p == null) return null!;
            return ToDtoExpr.Compile().Invoke(p);
        }
    }


}
