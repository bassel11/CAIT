﻿namespace Identity.Core.Enums
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
        Vote = 14            // تصويت
    }
}
