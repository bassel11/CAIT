namespace BuildingBlocks.Shared.Authorization
{
    // 1. هيكل بيانات الصلاحية الواحدة
    public class PermissionEntry
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool Allow { get; set; }

        public string ScopeName { get; set; } = string.Empty; // "Global", "Resource", "Parent"

        public string? ResourceTypeName { get; set; }
        public Guid? ResourceId { get; set; }

        public string? ParentResourceTypeName { get; set; }
        public Guid? ParentResourceId { get; set; }
    }

    // 2. الحاوية الرئيسية
    public class PermissionSnapshot
    {
        public Guid UserId { get; set; }

        // 0=None, 1=Predefined, 2=Custom
        public int UserPrivilageType { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public List<PermissionEntry> Permissions { get; set; } = new();

        /// <summary>
        /// الفحص الصارم للصلاحيات بناءً على القيود المخزنة لدى المستخدم
        /// </summary>
        public bool Has(string permissionName, Guid? resourceId = null, Guid? parentResourceId = null)
        {
            // 🛑 الحالة 0: رفض قاطع
            if (UserPrivilageType == 0) return false;

            // تصفية أولية (الاسم، الفعالية، السماحية)
            var candidates = Permissions.Where(p =>
                p.Name.Equals(permissionName, StringComparison.OrdinalIgnoreCase) &&
                p.IsActive &&
                p.Allow);

            if (!candidates.Any()) return false;

            // ✅ الحالة 1: Predefined (دائماً مقبول بمجرد وجود الصلاحية)
            if (UserPrivilageType == 1) return true;

            // 🚀 الحالة 2: Custom (التحقق الصارم)
            if (UserPrivilageType == 2)
            {
                // ---------------------------------------------------------
                // أ) الحالة العامة: لم يتم تمرير أي موارد (طلب عام)
                // ---------------------------------------------------------
                // هنا نطلب حصراً أن يمتلك المستخدم صلاحية Global
                if (resourceId == null && parentResourceId == null)
                {
                    return candidates.Any(p => p.ScopeName.Equals("Global", StringComparison.OrdinalIgnoreCase));
                }

                // ---------------------------------------------------------
                // ب) الحالة المحددة: تم تمرير موارد (تطبيق القيود الصارمة)
                // ---------------------------------------------------------
                return candidates.Any(p =>
                {
                    // 1. إذا كانت الصلاحية Global فهي مقبولة دائماً
                    if (p.ScopeName.Equals("Global", StringComparison.OrdinalIgnoreCase)) return true;

                    // 2. التحقق من قيد ResourceId (إذا كان مسجلاً في الصلاحية)
                    if (p.ResourceId.HasValue)
                    {
                        // الشرط: يجب أن يكون المطور قد مرر ResourceId، ويجب أن يطابق بدقة
                        if (!resourceId.HasValue || resourceId.Value != p.ResourceId.Value)
                        {
                            return false; // فشل القيد
                        }
                    }

                    // 3. التحقق من قيد ParentResourceId (إذا كان مسجلاً في الصلاحية)
                    if (p.ParentResourceId.HasValue)
                    {
                        // الشرط: يجب أن يكون المطور قد مرر ParentResourceId، ويجب أن يطابق بدقة
                        if (!parentResourceId.HasValue || parentResourceId.Value != p.ParentResourceId.Value)
                        {
                            return false; // فشل القيد
                        }
                    }

                    // إذا وصلنا هنا، فهذا يعني أن جميع القيود الموجودة في هذا السجل قد تم استيفاؤها بنجاح
                    return true;
                });
            }

            return false;
        }
    }
}