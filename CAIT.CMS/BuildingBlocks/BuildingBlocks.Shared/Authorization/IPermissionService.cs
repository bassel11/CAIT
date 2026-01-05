namespace BuildingBlocks.Shared.Authorization
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(
            Guid userId,
            string permissionName,
            Guid? resourceId = null);

        // دالة لإبطال الكاش عند وصول حدث التحديث
        Task InvalidateCacheAsync(Guid userId);
    }
}
