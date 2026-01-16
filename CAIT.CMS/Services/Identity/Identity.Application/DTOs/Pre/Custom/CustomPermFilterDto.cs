using Identity.Application.Common;
using Identity.Core.Enums;

namespace Identity.Application.DTOs.Pre.Custom
{
    public class CustomPermFilterDto : BaseFilterDto
    {
        public Guid? RoleId { get; set; }
        public Guid? PermissionId { get; set; }
        public ScopeType? Scope { get; set; }
        public ResourceType? ResourceType { get; set; }
        public Guid? ResourceId { get; set; }
        public ResourceType? ParentResourceType { get; set; }
        public Guid? ParentResourceId { get; set; }
        public bool? Allow { get; set; }
    }
}
