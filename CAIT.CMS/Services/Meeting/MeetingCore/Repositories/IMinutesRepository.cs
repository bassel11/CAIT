using MeetingCore.Entities;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingCore.Repositories
{
    public interface IMinutesRepository
    {
        // وحدة العمل لحفظ التغييرات
        IUnitOfWork UnitOfWork { get; }

        // 1. الإضافة (لا تحتاج أي جلب)
        Task AddAsync(MinutesOfMeeting mom, CancellationToken ct = default);

        // 2. التحقق الخفيف (Simple Check)
        // الاستخدام: عند الـ Submit, Reject, Publish أو التحقق من الوجود
        // السبب: نحتاج فقط معرفة الـ Status والـ Id، لا يهمنا القرارات أو المهام هنا.
        Task<MinutesOfMeeting?> GetByMeetingIdSimpleAsync(MeetingId meetingId, CancellationToken ct = default);

        // 3. سياق القرارات (Decision Context)
        // الاستخدام: عند تنفيذ AddDecisionDraftCommand
        // السبب: نحتاج تحميل قائمة Decisions لحساب الترتيب (SortOrder = Count + 1).
        Task<MinutesOfMeeting?> GetWithDecisionsByMeetingIdAsync(MeetingId meetingId, CancellationToken ct = default);

        // 4. سياق المهام (Action Item Context)
        // الاستخدام: عند تنفيذ AddActionItemDraftCommand
        // السبب: نحتاج تحميل قائمة ActionItems لحساب الترتيب.
        Task<MinutesOfMeeting?> GetWithActionItemsByMeetingIdAsync(MeetingId meetingId, CancellationToken ct = default);

        // 5. سياق المرفقات (Attachment Context)
        // الاستخدام: عند تنفيذ AddAttachmentCommand
        // السبب: للتأكد من عدم تكرار اسم الملف أو تجاوز العدد المسموح (Business Invariants).
        Task<MinutesOfMeeting?> GetWithAttachmentsByMeetingIdAsync(MeetingId meetingId, CancellationToken ct = default);

        // 6. سياق النسخ والمحتوى (Versioning Context)
        // الاستخدام: عند تنفيذ UpdateMoMContentCommand
        // السبب: عند تحديث المحتوى، نقوم بنقل المحتوى القديم إلى قائمة _versions، لذا يجب أن تكون هذه القائمة محملة في الذاكرة.
        Task<MinutesOfMeeting?> GetWithVersionsByMeetingIdAsync(MeetingId meetingId, CancellationToken ct = default);

        // 7. السياق الكامل (Full Graph Context)
        // الاستخدام: عند تنفيذ ApproveMoMCommand
        // السبب: عند الاعتماد، نطلق حدثاً (Integration Event) يجب أن يحتوي على *كل* القرارات والمهام لإرسالها للأنظمة الأخرى.
        Task<MinutesOfMeeting?> GetFullGraphByMeetingIdAsync(MeetingId meetingId, CancellationToken ct = default);

        // دوال مساعدة
        Task<bool> ExistsForMeetingAsync(MeetingId meetingId, CancellationToken ct = default);
    }
}