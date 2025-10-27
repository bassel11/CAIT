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
            Resource = p.Resource.ToString(),
            Action = p.Action.ToString(),
            IsGlobal = p.IsGlobal,
            IsActive = p.IsActive
        };

        // helper if you need to map an already-loaded entity
        public static PermissionDto ToDto(Permission p) => ToDtoExpr.Compile().Invoke(p);



        //public static PermissionDto ToDto(Permission p)
        //{
        //    return new PermissionDto
        //    {
        //        Id = p.Id,
        //        Name = p.Name,
        //        Description = p.Description,
        //        Resource = p.Resource.ToString(),
        //        Action = p.Action.ToString(),
        //        IsGlobal = p.IsGlobal,
        //        IsActive = p.IsActive
        //    };
        //}
    }


}
