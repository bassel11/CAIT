using Identity.Core.Enums;

namespace Identity.Core.Entities
{
    public class UserPermissionAssignment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;
        public PermissionScopeType ScopeType { get; set; } = PermissionScopeType.Global;
        //public Guid? CommitteeId { get; set; }
        public Guid? ResourceId { get; set; }
        public Resource? Resource { get; set; }
        public bool Allow { get; set; } = true;  // false = explicit deny
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }  // for time-limited grants (guest accounts)
    }
}
