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

        public async Task<PermissionSnapshot> BuildSnapshotAsync(Guid userId, string? securityStamp = null)
        {
            // 1️⃣ جلب البيانات (تمرير الـ securityStamp للطبقة الأدنى)
            // سيتم التحقق من البصمة داخل GetSnapshotsAsync
            var data = await _query.GetSnapshotsAsync(userId, securityStamp);

            // 2️⃣ تجهيز الحاوية النهائية (باقي الكود كما هو تماماً)
            var snapshot = new PermissionSnapshot
            {
                UserId = userId,
                UserPrivilageType = (int)data.UserPrivilageType,
                GeneratedAt = DateTime.UtcNow
            };

            // 3️⃣ Mapping
            if (data.Permissions == null || !data.Permissions.Any())
                return snapshot;

            snapshot.Permissions = data.Permissions.Select(p => new PermissionEntry
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