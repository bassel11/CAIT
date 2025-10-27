using Identity.Application.Common;
using Identity.Core.Enums;

namespace Identity.Application.DTOs.RolePermissions
{
    public class PermissionByRoleFilterDto : BaseFilter
    {
        // فلتر بواسطة Resource enum
        public ResourceType? Resource { get; set; }

        // فلتر بواسطة Action enum
        public ActionType? Action { get; set; }

        // فلتر على النشاط
        public bool? IsActive { get; set; }

        // فلتر على الصلاحية العامة
        public bool? IsGlobal { get; set; }

    }
}
