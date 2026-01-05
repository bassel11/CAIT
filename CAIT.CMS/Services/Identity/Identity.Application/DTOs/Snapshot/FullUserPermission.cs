using Identity.Core.Enums;

namespace Identity.Application.DTOs.Snapshot
{
    public class FullUserPermission
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        // Scope (غالبًا غير Nullable)
        public ScopeType Scope { get; set; }
        public string ScopeName { get; set; } = string.Empty;

        // Resource (Nullable ككتلة واحدة)
        public ResourceType? ResourceType { get; set; }
        public string? ResourceTypeName { get; set; }
        public Guid? ResourceId { get; set; }

        // Parent Resource (Nullable ككتلة واحدة)
        public ResourceType? ParentResourceType { get; set; }
        public string? ParentResourceTypeName { get; set; }
        public Guid? ParentResourceId { get; set; }

        // Final decision
        public bool Allow { get; set; }
    }

}
