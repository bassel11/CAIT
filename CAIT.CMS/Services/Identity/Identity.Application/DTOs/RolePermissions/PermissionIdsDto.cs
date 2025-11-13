using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs.RolePermissions
{
    public class PermissionIdsDto
    {
        [Required]
        public Guid PermissionId { get; set; }
    }
}
