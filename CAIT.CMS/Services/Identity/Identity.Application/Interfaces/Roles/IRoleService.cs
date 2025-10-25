using Identity.Application.Common;
using Identity.Application.DTOs.Roles;

namespace Identity.Application.Interfaces.Roles
{
    public interface IRoleService
    {
        Task<PagedResult<RoleDto>> GetRolesAsync(RoleFilterDto filter);
        Task<RoleDto?> GetByIdAsync(Guid id);
        Task<(bool Success, string? Error)> CreateAsync(RoleCreateDto dto);
        Task<(bool Success, string? Error)> UpdateAsync(Guid id, RoleUpdateDto dto);
        Task<(bool Success, string? Error)> DeleteAsync(Guid id);
    }
}
