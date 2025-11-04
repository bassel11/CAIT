using Identity.Application.Common;
using Identity.Core.Enums;

namespace Identity.Application.DTOs.Permissions
{
    public class PermissionFilterDto : BaseFilterDto
    {

        // فلتر بواسطة Resource enum (قيمة اسمية أو رقمية)
        public ResourceType? Resource { get; set; }

        // فلتر بواسطة Action enum
        public ActionType? Action { get; set; }

        // فلتر على النشاط/التفعيل
        public bool? IsActive { get; set; }

        // فلتر على IsGlobal
        public bool? IsGlobal { get; set; }
    }
}
