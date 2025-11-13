using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs.Pre.Custom
{
    public class RemoveCustomPermsDto
    {
        [Required]
        public Guid UserRolePermResoId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid RoleId { get; set; }

        [Required]
        public Guid PermissionId { get; set; }
    }
}
