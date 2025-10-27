using Identity.Application.DTOs.Permissions;
using Identity.Application.DTOs.RolePermissions;

namespace Identity.Application.Interfaces.RolePermissions
{
    public interface IRolePermissionService
    {
        Task<bool> AssignPermissionsToRoleAsync(AssignPermissionsToRoleDto dto);
        Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(Guid roleId, PermissionByRoleFilterDto? filter = null);
    }
}
