using Identity.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Identity.Core.Entities
{
    public class Permission
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }

        [Required]
        public ResourceType Resource { get; set; } // e.g. "Committee", "Meeting", "Task"

        [Required]
        public ActionType Action { get; set; } // e.g. "Create", "View", "Update", "Delete", "Approve"
        public string Description { get; set; } = string.Empty;
        public bool IsGlobal { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
