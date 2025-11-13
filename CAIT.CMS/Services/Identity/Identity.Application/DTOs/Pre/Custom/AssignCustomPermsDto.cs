using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs.Pre.Custom
{
    public class AssignCustomPermsDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid RoleId { get; set; }

        [Required]
        public List<CustomPermsDto> Permissions { get; set; } = new();

    }
}
