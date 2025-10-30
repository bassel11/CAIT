using Identity.Core.Enums;

namespace Identity.Application.DTOs.RolePermissions
{
    public class RolePermissionDto
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        // نطاق الصلاحية (Global, Committee, ResourceInstance)
        public PermissionScopeType ScopeType { get; set; } = PermissionScopeType.Global;

        // معرف اللجنة عند استخدام ScopeType = Committee
        public Guid? CommitteeId { get; set; }

        // معرف المورد عند استخدام ScopeType = ResourceInstance
        public Guid? ResourceId { get; set; }

        // السماح أو الرفض (Allow / Deny)
        public bool Allow { get; set; } = true;

        // بيانات إضافية للعرض أو التدقيق (اختياري)
        public DateTime? CreatedAt { get; set; }
    }
}
