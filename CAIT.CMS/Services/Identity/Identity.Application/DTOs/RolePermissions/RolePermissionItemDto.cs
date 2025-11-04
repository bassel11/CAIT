using Identity.Core.Enums;

namespace Identity.Application.DTOs.RolePermissions
{
    public class RolePermissionItemDto
    {
        public Guid PermissionId { get; set; }

        // Global = على مستوى النظام, ResourceType = ضمن كيان معين, ResourceInstance = على كيان معين
        public PermissionScopeType ScopeType { get; set; } = PermissionScopeType.Global;

        // عند ScopeType = Committee
        // public Guid? CommitteeId { get; set; }

        // عند ScopeType = ResourceInstance
        public Guid? ResourceId { get; set; }

        // السماح أو الرفض
        public bool Allow { get; set; } = true;
    }
}
