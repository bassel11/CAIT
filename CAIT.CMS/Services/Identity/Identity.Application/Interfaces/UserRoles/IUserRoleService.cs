using Identity.Core.Entities;

namespace Identity.Application.Interfaces.UserRoles
{
    public interface IUserRoleService
    {
        Task<bool> AssignUserRoleAsync(Guid userId, string roleName);
        Task<bool> RemoveUserRoleAsync(Guid userId, string roleName);
        Task<IList<string>> GetUserRolesAsync(Guid userId);
        Task<IList<ApplicationUser>> GetUsersByRoleAsync(string roleName);


        #region Multiple
        Task<(bool Success, string Message)> AssignUserRolesAsync(Guid userId, List<string> roles);
        Task<bool> RemoveUserRolesAsync(Guid userId, List<string> roles);
        Task<bool> AssignUsersToRoleAsync(string roleName, List<Guid> userIds);
        #endregion


    }
}
