using MeetingCore.ValueObjects.AIGeneratedContentVO;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingCore.Entities
{
    public class AIGeneratedContent : Entity<AIContentId>
    {
        // الربط بالاجتماع
        public MeetingId MeetingId { get; private set; } = default!;

        // نوع المحتوى
        public AIContentType ContentType { get; private set; }

        // 2. تفاصيل العملية (مهمة جداً للتدقيق المستقبلي وتحسين النتائج)
        public string Prompt { get; private set; } = default!;
        public string GeneratedText { get; private set; } = default!;

        // 3. حقل إضافي (Enterprise Feature): معرفة الموديل المستخدم
        // يساعدك لاحقاً لتعرف: هل GPT-4 أعطى نتائج أفضل من GPT-3.5؟
        public string ModelUsed { get; private set; } = default!; // e.g., "gpt-4-turbo", "claude-3-opus"

        // 4. التغذية الراجعة (Feedback Loop)
        // هل قام المستخدم باعتماد هذا النص؟ مفيد جداً لتحليل جودة الـ AI
        public bool IsApplied { get; private set; }

        // EF Core Constructor
        private AIGeneratedContent() { }

        // Factory Constructor (Internal)
        // لا يتم إنشاؤه إلا من خلال Meeting Aggregate
        internal AIGeneratedContent(
            MeetingId meetingId,
            AIContentType contentType,
            string prompt,
            string generatedText,
            string modelUsed,
            UserId createdBy)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new DomainException("Prompt cannot be empty.");

            if (string.IsNullOrWhiteSpace(generatedText))
                throw new DomainException("Generated text cannot be empty.");

            Id = AIContentId.Of(Guid.NewGuid());
            MeetingId = meetingId;
            ContentType = contentType;
            Prompt = prompt;
            GeneratedText = generatedText;
            ModelUsed = modelUsed;
            IsApplied = false; // افتراضياً لم يتم تطبيقه بعد

            // Audit
            CreatedBy = createdBy.Value.ToString();
            CreatedAt = DateTime.UtcNow;
        }

        // Behavior: تمييز المحتوى كـ "مطبق"
        public void MarkAsApplied()
        {
            IsApplied = true;
        }
    }
}
