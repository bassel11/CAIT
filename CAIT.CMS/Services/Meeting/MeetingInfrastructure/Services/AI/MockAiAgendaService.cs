using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Interfaces.AI;

namespace MeetingInfrastructure.Services.AI
{
    public class MockAiAgendaService : IAiAgendaService
    {
        public async Task<Result<List<string>>> GenerateAgendaSuggestionsAsync(string meetingPurpose, string meetingTitle)
        {
            // 1. محاكاة التأخير الشبكي (Latency) وكأننا نتصل بـ Azure OpenAI
            // ننتظر عشوائياً بين 1 إلى 2.5 ثانية
            await Task.Delay(Random.Shared.Next(1000, 2500));

            // 2. دمج النصوص للبحث عن كلمات مفتاحية
            var input = $"{meetingTitle} {meetingPurpose}".ToLower();
            List<string> suggestions = new();

            // 3. منطق ذكي "بسيط" (Rule-Based) لتقديم مقترحات واقعية
            if (input.Contains("ميزانية") || input.Contains("budget") || input.Contains("مالية") || input.Contains("finance"))
            {
                suggestions = new List<string>
                {
                    "استعراض تقرير المصروفات للربع الحالي",
                    "مناقشة العجز/الفائض في الميزانية",
                    "الموافقة على المخصصات المالية للمشاريع الجديدة",
                    "مراجعة تقارير التدقيق المالي",
                    "توصيات خفض التكاليف"
                };
            }
            else if (input.Contains("مشروع") || input.Contains("project") || input.Contains("تطور") || input.Contains("status"))
            {
                suggestions = new List<string>
                {
                    "مراجعة حالة المشاريع القائمة",
                    "تحليل المخاطر والمعوقات",
                    "تحديث الجدول الزمني للتسليم",
                    "تخصيص الموارد البشرية للمشاريع",
                    "اعتماد إغلاق المشاريع المنتهية"
                };
            }
            else if (input.Contains("توظيف") || input.Contains("hr") || input.Contains("موارد بشرية") || input.Contains("hiring"))
            {
                suggestions = new List<string>
                {
                    "مراجعة خطة القوى العاملة",
                    "مقابلة المرشحين للمناصب القيادية",
                    "مناقشة سياسات العمل عن بعد",
                    "اعتماد الترقيات السنوية",
                    "تقرير رضا الموظفين"
                };
            }
            else if (input.Contains("ai") || input.Contains("ذكاء") || input.Contains("tech") || input.Contains("تقنية"))
            {
                suggestions = new List<string>
                {
                    "استعراض استراتيجية التحول الرقمي",
                    "تقييم أدوات الذكاء الاصطناعي الجديدة",
                    "أمن المعلومات وحماية البيانات",
                    "جاهزية البنية التحتية السحابية",
                    "اعتماد موردي التقنية الجدد"
                };
            }
            else
            {
                // مقترحات عامة (Fallback)
                suggestions = new List<string>
                {
                    "الافتتاح والترحيب",
                    "التصديق على محضر الاجتماع السابق",
                    "متابعة تنفيذ القرارات السابقة",
                    "مناقشة البنود الرئيسية",
                    "ما يستجد من أعمال",
                    "تحديد موعد الاجتماع القادم والخاتمة"
                };
            }

            // 4. إرجاع النتيجة
            return Result<List<string>>.Success(suggestions, "Mock AI: Agenda generated based on keywords.");
        }
    }
}
