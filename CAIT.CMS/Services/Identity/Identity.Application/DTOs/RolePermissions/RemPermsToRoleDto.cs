using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs.RolePermissions
{
    public class RemPermsToRoleDto
    {
        [Required]
        public Guid RoleId { get; set; }

        [Required]
        public Guid PermissionId { get; set; }
    }
}
