using MeetingCore.Entities;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingCore.Repositories
{
    public interface IMeetingRepository
    {
        IUnitOfWork UnitOfWork { get; }

        // إضافة الاجتماع للذاكرة (Tracking)
        Task AddAsync(Meeting meeting, CancellationToken cancellationToken = default);

        // دوال الجلب المتخصصة (Granular Fetching)

        // 1. خفيفة: للتحقق من الوجود أو تعديل بيانات بسيطة
        Task<Meeting?> GetByIdAsync(MeetingId id, CancellationToken cancellationToken = default);

        // 2. مع الأجندة: لعمليات إضافة/ترتيب البنود
        Task<Meeting?> GetWithAgendaAsync(MeetingId id, CancellationToken cancellationToken = default);

        // 3. مع الحضور: لعمليات الدعوة والحضور
        Task<Meeting?> GetWithAttendeesAsync(MeetingId id, CancellationToken cancellationToken = default);

        // 4. مع المحضر: لعمليات التوثيق
        Task<Meeting?> GetWithMinutesAsync(MeetingId id, CancellationToken cancellationToken = default);

        Task<Meeting?> GetForSchedulingAsync(MeetingId id, CancellationToken cancellationToken = default);

        IQueryable<Meeting> GetTableNoTracking();

        // ✅ الدالة الجديدة للتحقق من التعارض
        Task<bool> HasConflictAsync(
            DateTime startDate,
            DateTime endDate,
            string roomName,
            MeetingId? excludeMeetingId = null, // لتجاوز الاجتماع الحالي عند التعديل
            CancellationToken cancellationToken = default);
    }
}
