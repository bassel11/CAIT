using Identity.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs.Pre.Custom
{
    public class CustomPermsDto
    {
        [Required]
        public Guid PermissionId { get; set; }

        [Required]
        public ScopeType Scope { get; set; } = ScopeType.Global;

        public ResourceType ResourceType { get; set; } = ResourceType.None;
        public Guid? ResourceId { get; set; }

        public ResourceType ParentResourceType { get; set; } = ResourceType.None;
        public Guid? ParentResourceId { get; set; }
        public bool Allow { get; set; } = true;
    }
}
