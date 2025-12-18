namespace BuildingBlocks.Shared.Authorization
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(Guid userId, string permissionName, Guid? resourceId = null);
    }
}
