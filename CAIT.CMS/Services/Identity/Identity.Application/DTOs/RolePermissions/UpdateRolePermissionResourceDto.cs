using Identity.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs.RolePermissions
{
    public class UpdateRolePermissionResourceDto
    {
        [Required]
        public Guid RoleId { get; set; }

        [Required]
        public Guid PermissionId { get; set; }

        [Required]
        public PermissionScopeType ScopeType { get; set; }

        [Required]
        public Guid ResourceId { get; set; }

        public bool? Allow { get; set; }
    }
}
