using Identity.Application.DTOs.RolePermissions;

namespace Identity.Application.Interfaces.RolePermissions
{
    public interface IRolePremissionService
    {
        Task<bool> AsgnPermsToRoleAsync(AsgnPermsToRoleDto dto);
        Task<IEnumerable<PermsDetailsDto>> GetPermsByRoleAsync(PermsByRoleFilterDto? filter = null);
        Task<bool> RemovePermissionsOfRoleAsync(RemPermsToRoleDto dto);
    }
}
