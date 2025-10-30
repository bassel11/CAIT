using Identity.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Identity.Core.Entities
{
    public class Resource
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Logical type of the resource (e.g., Committee, Meeting, Document, Task, Decision)
        /// </summary>
        [Required]
        public ResourceType ResourceType { get; set; }

        /// <summary>
        /// The Committee this resource belongs to (if applicable)
        /// </summary>
        public Guid? CommitteeId { get; set; }

        /// <summary>
        /// ReferenceId points to the actual domain entity record (e.g. Meeting.Id, Document.Id)
        /// </summary>
        [Required]
        public Guid ReferenceId { get; set; }

        /// <summary>
        /// Optional display name for convenience in logs or audits
        /// </summary>
        [MaxLength(300)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Metadata for search or audit — e.g. version, owner, etc.
        /// </summary>
        public string? MetadataJson { get; set; }

        /// <summary>
        /// Audit fields
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation for permission scoping
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public virtual ICollection<UserPermissionAssignment> UserPermissionAssignments { get; set; } = new List<UserPermissionAssignment>();
    }
}
