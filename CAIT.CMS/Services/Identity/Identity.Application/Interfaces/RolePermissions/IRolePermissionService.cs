using Identity.Application.DTOs.Permissions;
using Identity.Application.DTOs.RolePermissions;

namespace Identity.Application.Interfaces.RolePermissions
{
    public interface IRolePermissionService
    {
        Task<bool> AssignPermissionsToRoleAsync(AssignPermissionsToRoleDto dto);
        Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(PermissionByRoleFilterDto? filter = null);
        Task<IEnumerable<RolePermissionDetailsDto>> GetRolePermissionsWithResourcesAsync(PermissionByRoleFilterDto? filter = null);
    }
}
