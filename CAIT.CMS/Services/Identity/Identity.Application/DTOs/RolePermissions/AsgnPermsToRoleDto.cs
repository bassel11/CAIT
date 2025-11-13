using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs.RolePermissions
{
    public class AsgnPermsToRoleDto
    {
        [Required]
        public Guid RoleId { get; set; }

        [Required]
        public List<PermsDto> Permissions { get; set; } = new();
    }
}
