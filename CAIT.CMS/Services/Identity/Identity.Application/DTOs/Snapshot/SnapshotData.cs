using Identity.Core.Enums;

namespace Identity.Application.DTOs.Snapshot
{
    // كلاس مساعد لنقل البيانات من Infrastructure إلى Application
    public class SnapshotData
    {
        public PrivilageType UserPrivilageType { get; set; }
        public IReadOnlyList<FullUserPermission> Permissions { get; set; } = Array.Empty<FullUserPermission>();
    }
}
