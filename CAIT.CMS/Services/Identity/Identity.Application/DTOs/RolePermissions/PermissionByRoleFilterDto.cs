using Identity.Application.Common;
using Identity.Core.Enums;

namespace Identity.Application.DTOs.RolePermissions
{
    public class PermissionByRoleFilterDto : BaseFilter
    {
        //  لتحديد الدور المطلوب فحص صلاحياته
        public Guid? RoleId { get; set; }

        //  فلتر بواسطة نوع المورد (Committee, Meeting, Task, ... إلخ)
        public ResourceType? ResourceType { get; set; }

        //  فلتر بواسطة نوع الإجراء (Create, View, Edit, Delete, ... إلخ)
        public ActionType? Action { get; set; }

        //  فلتر على نطاق الصلاحية (Global, Committee, ResourceInstance)
        public PermissionScopeType? ScopeType { get; set; }

        //  فلتر على لجنة معينة عند وجود ScopeType = Committee
        public Guid? CommitteeId { get; set; }

        //  فلتر على كيان مورد معين (Resource Instance)
        public Guid? ResourceId { get; set; }

        //  فلتر على السماح أو الرفض
        public bool? Allow { get; set; }

        //  فلتر على النشاط (هل الصلاحية مفعلة أو لا)
        public bool? IsActive { get; set; }

        //  فلتر على الصلاحيات العامة فقط
        public bool? IsGlobal { get; set; }

    }
}
