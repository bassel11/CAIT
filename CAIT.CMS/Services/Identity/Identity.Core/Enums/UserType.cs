namespace Identity.Core.Enums
{
    public enum UserType
    {
        InternalEmployee = 1,   // موظف تابع للجهة
        SystemAccount = 2,      // حسابات خدمات خلفية (Service Accounts)
        ExternalUser = 3,     // مورد خارجي / استشاري
        Guest = 4,              // ضيف (صلاحيات محدودة جداً)

    }
}
