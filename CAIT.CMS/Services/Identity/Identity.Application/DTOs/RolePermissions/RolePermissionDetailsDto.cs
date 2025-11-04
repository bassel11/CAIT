using Identity.Core.Enums;

namespace Identity.Application.DTOs.RolePermissions
{
    public class RolePermissionDetailsDto
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ResourceType PermissionResourceType { get; set; }
        public string PermissionResourceTypeName { get; set; } = string.Empty;

        public ActionType Action { get; set; }
        public string ActionName { get; set; } = string.Empty;

        public PermissionScopeType PermissionScopeType { get; set; }
        public string PermissionScopeTypeName { get; set; } = string.Empty;

        public bool IsGlobal { get; set; }
        public bool IsActive { get; set; }
        public bool Allow { get; set; }
        public DateTime CreatedAt { get; set; }

        // 🟢 Resource Information
        public Guid? ResourceId { get; set; }
        public Guid? ResourceReferenceId { get; set; }        // <--- تمت إضافتها الآن
        public ResourceType? Res_Type { get; set; } // <--- نوع المورد (من جدول Resource)
        public string? Res_TypeName { get; set; }   // <--- نص النوع
        public string? ResourceDisplayName { get; set; }
        public ResourceType? ResourceParentType { get; set; }
        public string? ResourceParentTypeName { get; set; }
        public Guid? ResourceParentReferenceId { get; set; }
    }
}
