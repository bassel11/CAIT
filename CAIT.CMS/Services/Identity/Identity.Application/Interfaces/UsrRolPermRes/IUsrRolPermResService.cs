using Identity.Application.DTOs.Pre.Custom;

namespace Identity.Application.Interfaces.UsrRolPermRes
{
    public interface IUsrRolPermResService
    {
        #region Interface Actions

        Task<bool> AssignCustomPermsAsync(AssignCustomPermsDto dto);
        Task<IEnumerable<CustomPermsDetailsDto>> GetCustomPermsAsync(Guid UserId, CustomPermFilterDto? filter = null);
        Task<bool> RemoveCustomPermsAsync(RemoveCustomPermsDto dto);
        Task<bool> HasCustomPermissionsAsync(Guid userId);

        #endregion
    }
}
