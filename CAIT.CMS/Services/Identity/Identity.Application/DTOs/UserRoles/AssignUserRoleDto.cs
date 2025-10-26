using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs.UserRoles
{
    public class AssignUserRoleDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string RoleName { get; set; }
    }
}
