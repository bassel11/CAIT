

namespace BuildingBlocks.Shared.Authorization
{
    //public class PermissionSnapshot
    //{
    //    public Guid UserId { get; set; }
    //    public List<string> Permissions { get; set; } = new();
    //    public bool Has(string permission, Guid? resourceId = null)
    //    {
    //        // لو أردت دعم ResourceId
    //        return Permissions.Contains(permission);
    //    }
    //}

    // 1. التفاصيل الكاملة للصف الواحد (Rich Entry)
    public class PermissionEntry
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool Allow { get; set; }

        // Enums
        //public ScopeType Scope { get; set; }
        public string ScopeName { get; set; } = string.Empty;

        // Resource
        //public ResourceType? ResourceType { get; set; }
        public string? ResourceTypeName { get; set; }
        public Guid? ResourceId { get; set; }

        // Parent Resource
        //public ResourceType? ParentResourceType { get; set; }
        public string? ParentResourceTypeName { get; set; }
        public Guid? ParentResourceId { get; set; }
    }

    // 2. الحاوية الرئيسية (هذا ما يتم تخزينه في الكاش وإرساله عبر الشبكة)
    public class PermissionSnapshot
    {
        public Guid UserId { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        // القائمة الكاملة
        public List<PermissionEntry> Permissions { get; set; } = new();

        // 🧠 المنطق الذكي للفحص (يستخدم كل الحقول)
        public bool Has(string permissionName, Guid? resourceId = null)
        {
            // نبحث عن الصلاحية بالاسم وتكون فعالة ومسموحة
            var entries = Permissions.Where(p =>
                p.Name.Equals(permissionName, StringComparison.OrdinalIgnoreCase) &&
                p.IsActive &&
                p.Allow);

            if (!entries.Any()) return false;

            // إذا لم نمرر ResourceId، يكفي وجود أي entry (Global or specific)
            if (resourceId == null) return true;

            // إذا مررنا ResourceId، يجب التحقق من النطاق
            return entries.Any(p =>
                //p.Scope == ScopeType.Global || // صلاحية عامة
                p.ResourceId == resourceId ||  // صلاحية خاصة بهذا العنصر
                (p.ParentResourceId.HasValue && /* منطق الوراثة هنا إن وجد */ false)
            );
        }
    }

}
