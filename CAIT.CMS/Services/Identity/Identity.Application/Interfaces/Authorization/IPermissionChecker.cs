namespace Identity.Application.Interfaces.Authorization
{
    public interface IPermissionChecker
    {
        Task<bool> HasPermissionAsync(Guid userId, string permissionName, Guid? resourceId = null);
        void InvalidateCache(Guid userId);
    }

}
