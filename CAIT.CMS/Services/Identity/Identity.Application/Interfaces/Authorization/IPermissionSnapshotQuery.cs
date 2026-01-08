using Identity.Application.DTOs.Snapshot;

namespace Identity.Application.Interfaces.Authorization
{
    public interface IPermissionSnapshotQuery
    {
        //Task<IReadOnlyList<FullUserPermission>> GetSnapshotsAsync(Guid userId);
        Task<SnapshotData> GetSnapshotsAsync(Guid userId);

    }
}
