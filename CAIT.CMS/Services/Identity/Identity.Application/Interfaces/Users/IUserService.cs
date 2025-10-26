using Identity.Application.Common;
using Identity.Application.DTOs.Users;

namespace Identity.Application.Interfaces.Users
{
    public interface IUserService
    {
        Task<PagedResult<UserDto>> GetUsersAsync(UserFilterDto filter);
        Task<UserDto?> GetByIdAsync(Guid id);
        Task<(bool Success, string? Error)> UpdateAsync(Guid id, UserUpdateDto dto);
        Task<(bool Success, string? Error)> SoftDeleteAsync(Guid id);

        Task<(bool Success, string? Error)> DeactivateUserAsync(string userId);
    }
}
