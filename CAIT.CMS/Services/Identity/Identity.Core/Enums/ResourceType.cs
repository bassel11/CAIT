namespace Identity.Core.Enums
{
    public enum ResourceType
    {
        System = 0,          // إعدادات النظام العامة
        User = 1,            // إدارة المستخدمين
                             //    Role = 2,            // إدارة الأدوار
        Committee = 2,       // إدارة اللجان
        Meeting = 3,         // الاجتماعات
        Decision = 4,        // القرارات
        Task = 5,            // المهام
        Document = 6,        // المستندات
        Audit = 7,           // السجل التدقيقي
        Notification = 8,    // التنبيهات
        Integration = 9,    // تكامل مع أنظمة خارجية
        Settings = 10        // إعدادات النظام
    }
}
