namespace Identity.Application.Interfaces.Authorization
{
    public interface IPermissionCacheInvalidator
    {
        Task InvalidateUserPermissionsByRoleAsync(Guid roleId);
        Task InvalidateUserPermissionsByUserAsync(Guid userId);
    }
}
