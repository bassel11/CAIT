using BuildingBlocks.Shared.Authorization;
using Identity.Application.Interfaces.Authorization;

namespace Identity.Infrastructure.Services.Authorization
{
    public sealed class PermissionSnapshotService : IPermissionSnapshotService
    {
        private readonly IPermissionSnapshotQuery _query;

        public PermissionSnapshotService(IPermissionSnapshotQuery query)
        {
            _query = query;
        }

        public async Task<PermissionSnapshot> BuildSnapshotAsync(Guid userId)
        {
            // 1️⃣ جلب البيانات الداخلية
            var internalPermissions = await _query.GetSnapshotsAsync(userId);

            // 2️⃣ تجهيز الحاوية النهائية
            var snapshot = new PermissionSnapshot
            {
                UserId = userId,
                GeneratedAt = DateTime.UtcNow
            };

            // 3️⃣ Mapping (Internal DTO → Shared Contract)
            if (internalPermissions == null || !internalPermissions.Any())
                return snapshot;

            snapshot.Permissions = internalPermissions.Select(p => new PermissionEntry
            {
                Name = p.Name,
                Description = p.Description,
                IsActive = p.IsActive,
                Allow = p.Allow,

                ScopeName = p.ScopeName,

                ResourceTypeName = p.ResourceTypeName,
                ResourceId = p.ResourceId,

                ParentResourceTypeName = p.ParentResourceTypeName,
                ParentResourceId = p.ParentResourceId

            }).ToList();

            return snapshot;
        }
    }
}
