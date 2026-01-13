using BuildingBlocks.Shared.Authorization;

namespace Identity.Application.Interfaces.Authorization
{
    public interface IPermissionSnapshotService
    {
        Task<PermissionSnapshot> BuildSnapshotAsync(Guid userId, string? securityStamp = null);
    }
}
