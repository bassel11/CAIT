using Identity.Application.DTOs.Roles;
using Identity.Core.Entities;
using System.Linq.Expressions;

namespace Identity.Application.Mappers
{
    public static class RoleMapper
    {
        public static readonly Expression<Func<ApplicationRole, RoleDto>> ToDtoExpr = p => new RoleDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            CreatedAt = p.CreatedAt,
            UserCount = p.UserRoles.Count
        };

        // helper if you need to map an already-loaded entity
        public static RoleDto ToDto(ApplicationRole? p)
        {
            if (p == null) return null!;
            return ToDtoExpr.Compile().Invoke(p);
        }
    }
}
