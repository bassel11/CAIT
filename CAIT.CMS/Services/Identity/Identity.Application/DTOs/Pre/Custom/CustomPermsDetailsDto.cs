using Identity.Core.Enums;

namespace Identity.Application.DTOs.Pre.Custom
{
    public class CustomPermsDetailsDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public Guid PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public ScopeType Scope { get; set; }
        public string ScopeName { get; set; } = string.Empty;
        public ResourceType ResourceType { get; set; }
        public string ResourceTypeName { get; set; } = string.Empty;
        public Guid? ResourceId { get; set; }

        public ResourceType ParentResourceType { get; set; }
        public string ParentResourceTypeName { get; set; } = string.Empty;
        public Guid? ParentResourceId { get; set; }
        public bool Allow { get; set; }
    }
}
