using Identity.Application.Common;
using Identity.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs.RolePermissions
{
    public class PermsByRoleFilterDto : BaseFilterDto
    {
        [Required]
        public Guid RoleId { get; set; }
        public string? PermissionName { get; set; }
        public string? Description { get; set; }
        public ResourceType? ResourceType { get; set; }
        public ActionType? Action { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsGlobal { get; set; }
    }
}
