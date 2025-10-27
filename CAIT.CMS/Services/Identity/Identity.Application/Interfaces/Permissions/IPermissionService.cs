using Identity.Application.DTOs.Permissions;

namespace Identity.Application.Interfaces.Permissions
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionDto>> GetAllAsync();
        Task<PermissionDto> GetByIdAsync(Guid id);
        Task<PermissionDto> CreateAsync(CreatePermissionDto dto);
        Task<PermissionDto> UpdateAsync(Guid id, UpdatePermissionDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
