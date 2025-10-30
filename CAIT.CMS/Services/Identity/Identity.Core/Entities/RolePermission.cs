using Identity.Core.Enums;

namespace Identity.Core.Entities
{
    public class RolePermission
    {
        public Guid RoleId { get; set; }
        public ApplicationRole Role { get; set; } = null!;

        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;

        public PermissionScopeType ScopeType { get; set; } = PermissionScopeType.Global;
        public Guid? CommitteeId { get; set; }            // when ScopeType == Committee
        public Guid? ResourceId { get; set; }             // when ScopeType == ResourceInstance

        public bool Allow { get; set; } = true;           // support for explicit deny if needed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
