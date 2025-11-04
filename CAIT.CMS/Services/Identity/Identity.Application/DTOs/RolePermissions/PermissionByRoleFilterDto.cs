using Identity.Application.Common;
using Identity.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs.RolePermissions
{
    public class PermissionByRoleFilterDto : BaseFilterDto
    {

        [Required]
        public Guid? RoleId { get; set; }

        //  فلتر بواسطة نوع المورد (Committee, Meeting, Task, ... إلخ)
        public ResourceType? ResourceType { get; set; }

        //  فلتر بواسطة نوع الإجراء (Create, View, Edit, Delete, ... إلخ)
        public ActionType? Action { get; set; }

        //  فلتر على نطاق الصلاحية (Global, Committee, ResourceInstance)
        public PermissionScopeType? ScopeType { get; set; }

        public Guid? ResourceId { get; set; }

        public bool? Allow { get; set; }

        public bool? IsActive { get; set; }

        //  فلتر على الصلاحيات العامة فقط
        public bool? IsGlobal { get; set; }

    }
}
