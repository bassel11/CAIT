namespace MeetingCore.Enums.MeetingEnums
{
    public enum MeetingStatus
    {
        Draft = 1,          // مسودة (تحت الإنشاء)
        Scheduled = 2,      // مجدول (تم إرسال الدعوات)
        Rescheduled = 3,    // معاد جدولته
        Cancelled = 4,      // ملغي
        InProgress = 5,     // جاري الآن
        Completed = 6       // انتهى (بانتظار المحضر)

    }
}
