using Identity.Application.DTOs.Permissions;
using Identity.Core.Entities;

namespace Identity.Application.Mappers
{
    public static class PermissionMapper
    {
        public static PermissionDto ToDto(Permission p)
        {
            return new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Resource = p.Resource.ToString(),
                Action = p.Action.ToString(),
                IsGlobal = p.IsGlobal,
                IsActive = p.IsActive
            };
        }
    }


}
