using Identity.Core.Enums;

namespace Identity.Application.DTOs.Permissions
{
    public class PermissionFilterDto
    {
        // بحث نصي على الاسم أو الوصف
        public string? Search { get; set; }

        // فلتر بواسطة Resource enum (قيمة اسمية أو رقمية)
        public ResourceType? Resource { get; set; }

        // فلتر بواسطة Action enum
        public ActionType? Action { get; set; }

        // فلتر على النشاط/التفعيل
        public bool? IsActive { get; set; }

        // فلتر على IsGlobal
        public bool? IsGlobal { get; set; }

        // ترتيب: "name", "createdAt", "resource", "action" إلخ
        public string? SortBy { get; set; } = "name";

        // "asc" أو "desc"
        public string SortDir { get; set; } = "asc";

        // Paging defaults
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
