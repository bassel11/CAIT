namespace Identity.Core.Enums
{
    public enum ActionType
    {
        View = 0,            // عرض
        Create = 1,          // إنشاء
        Update = 2,          // تعديل
        Delete = 3,          // حذف
        Approve = 4,         // اعتماد
        Assign = 5,          // تعيين / إسناد
        Upload = 6,          // رفع ملف
        Download = 7,        // تنزيل
        Archive = 8,         // أرشفة
        Suspend = 9,         // تعليق
        Activate = 10,       // تفعيل
        Configure = 11,      // ضبط الإعدادات
        AuditView = 12,      // عرض سجل التدقيق
        Export = 13,         // تصدير بيانات
        Vote = 14,           // تصويت
        Register = 15,       // تسجيل 
        Deactivate = 16,     // إلغاء تفعيل
        Remove = 17,         // إزالة
        Cancel = 18,         // إلغاء
        Complete = 19,       // إتمام 
        Reschedule = 20,      // إعادة جدولة
        Generate = 21,         // توليد أو إنشاء
        CheckIn = 22,         // نحقق
        Confirm = 23,         // تأكيد
        ValidateQuorum = 24,  // تحقق النصاب
        Reject = 25,           // رفض أو إلغاء
        Submit = 26           // تأكيد
    }
}
