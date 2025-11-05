using Identity.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Identity.Core.Entities
{
    public class Resource
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Logical type of the resource (e.g., Committee, Meeting, Document, Task, Decision)
        [Required]
        public ResourceType ResourceType { get; set; }

        // The Committee this resource belongs to (if applicable)
        //public Guid? CommitteeId { get; set; }

        // ReferenceId points to the actual domain entity record (e.g. Meeting.Id, Document.Id)
        [Required]
        public Guid ReferenceId { get; set; }

        // المورد الأب (Parent) في حال انتماء المورد إلى لجنة مثلاً
        [MaxLength(100)]
        public ResourceType? ParentResourceType { get; set; } // "Committee"
        public Guid? ParentReferenceId { get; set; }


        // Optional display name for convenience in logs or audits
        [MaxLength(300)]
        public string? DisplayName { get; set; }

        // Metadata for search or audit — e.g. version, owner, etc.
        public string? MetadataJson { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation for permission scoping
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public virtual ICollection<UserPermissionAssignment> UserPermissionAssignments { get; set; } = new List<UserPermissionAssignment>();
    }
}
