using TaskCore.Enums;

namespace TaskInfrastructure.Data.Extensions
{
    internal class InitialData
    {
        public static IEnumerable<TaskItem> Tasks
        {
            get
            {
                // مهمة 1: تشغيلية (Operational) مرتبطة بلجنة فقط
                var task1 = TaskItem.Create(
                    TaskItemId.Of(Guid.NewGuid()),
                    TaskTitle.Of("إعداد الميزانية السنوية"),
                    TaskDescription.Of("تجميع تقارير الربع الرابع والتوقعات للعام القادم."),
                    TaskDeadline.Of(DateTime.UtcNow.AddDays(14)), // موعد نهائي بعد أسبوعين
                    TaskPriority.High,
                    TaskCategory.Operational,
                    CommitteeId.Of(Guid.NewGuid()) // لجنة وهمية
                );
                // تعيين موظف لهذه المهمة
                task1.AssignUser(
                    UserId.Of(Guid.NewGuid()),
                    "ahmed@cait.gov",
                    "Ahmed Ali"
                );

                // مهمة 2: استراتيجية (Strategic) ناتجة عن اجتماع
                var task2 = TaskItem.Create(
                    TaskItemId.Of(Guid.NewGuid()),
                    TaskTitle.Of("استراتيجية التحول الرقمي"),
                    TaskDescription.Of("صياغة خارطة طريق لنقل الخدمات إلى السحابة."),
                    TaskDeadline.Of(DateTime.UtcNow.AddMonths(1)),
                    TaskPriority.Medium,
                    TaskCategory.Strategic,
                    CommitteeId.Of(Guid.NewGuid()),
                    meetingId: MeetingId.Of(Guid.NewGuid()) // مرتبطة باجتماع
                );
                // إضافة ملاحظة أولية
                task2.AddNote(
                    UserId.Of(Guid.NewGuid()),
                    "بانتظار موافقة قسم الأمن السيبراني."
                );

                // مهمة 3: امتثال (Compliance) ناتجة عن قرار، وهي متأخرة (لغرض الاختبار)
                var task3 = TaskItem.Create(
                    TaskItemId.Of(Guid.NewGuid()),
                    TaskTitle.Of("تحديث سياسات الخصوصية"),
                    TaskDescription.Of("مراجعة وتحديث السياسات وفق اللوائح الجديدة."),
                    TaskDeadline.Of(DateTime.UtcNow.AddDays(-2)), // موعد فات (Overdue)
                    TaskPriority.High,
                    TaskCategory.Compliance,
                    CommitteeId.Of(Guid.NewGuid()),
                    decisionId: DecisionId.Of(Guid.NewGuid()) // مرتبطة بقرار
                );
                // تعيين موظف
                task3.AssignUser(
                    UserId.Of(Guid.NewGuid()),
                    "sara@cait.gov",
                    "Sara Ahmed"
                );

                return new List<TaskItem> { task1, task2, task3 };
            }
        }
    }
}