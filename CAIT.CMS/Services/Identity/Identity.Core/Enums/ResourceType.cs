namespace Identity.Core.Enums
{
    public enum ResourceType
    {
        System = 0,          // إعدادات النظام العامة
        User = 1,            // إدارة المستخدمين
        Role = 2,            // إدارة الأدوار
        Committee = 3,       // إدارة اللجان
        Meeting = 4,         // الاجتماعات
        Decision = 5,        // القرارات
        Task = 6,            // المهام
        Document = 7,        // المستندات
        Audit = 8,           // السجل التدقيقي
        Notification = 9,    // التنبيهات
        Integration = 10,    // تكامل مع أنظمة خارجية
        Settings = 11        // إعدادات النظام
    }
}
