using Identity.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Identity.Core.Entities
{
    public class UserRolePermReso
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public Guid RoleId { get; set; }
        public ApplicationRole Role { get; set; }
        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; }
        public ScopeType Scope { get; set; } = ScopeType.Global;

        public ResourceType ResourceType { get; set; } = ResourceType.None;
        public Guid? ResourceId { get; set; }

        public ResourceType ParentResourceType { get; set; } = ResourceType.None;
        public Guid? ParentResourceId { get; set; }
        public bool Allow { get; set; } = true;

    }
}
