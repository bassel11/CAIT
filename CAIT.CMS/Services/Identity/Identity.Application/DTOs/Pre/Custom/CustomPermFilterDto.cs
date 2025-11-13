using Identity.Application.Common;
using Identity.Core.Enums;

namespace Identity.Application.DTOs.Pre.Custom
{
    public class CustomPermFilterDto : BaseFilterDto
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public ScopeType Scope { get; set; } = ScopeType.Global;
        public ResourceType ResourceType { get; set; } = ResourceType.None;
        public Guid? ResourceId { get; set; }
        public ResourceType ParentResourceType { get; set; } = ResourceType.None;
        public Guid? ParentResourceId { get; set; }
        public bool Allow { get; set; }
    }
}
