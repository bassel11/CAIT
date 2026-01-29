using BuildingBlocks.Shared.Abstractions;
using MeetingCore.Entities;
using MeetingCore.Enums.MeetingEnums;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;
using MeetingInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeetingInfrastructure.Repositories
{
    public class MeetingRepository : IMeetingRepository
    {
        private readonly MeetingDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public MeetingRepository(MeetingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            // نربط وحدة العمل بنفس سياق قاعدة البيانات الحالي
            _unitOfWork = new UnitOfWork(context);
        }

        public IUnitOfWork UnitOfWork => _unitOfWork;

        public async Task AddAsync(Meeting meeting, CancellationToken cancellationToken = default)
        {
            await _context.Meetings.AddAsync(meeting, cancellationToken);
        }

        // تنفيذ استراتيجية الجلب المجزأ (Granular Loading) لتحسين الأداء

        public async Task<Meeting?> GetByIdAsync(MeetingId id, CancellationToken cancellationToken = default)
        {
            return await _context.Meetings
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        //public async Task<Meeting?> GetWithAgendaAsync(MeetingId id, CancellationToken cancellationToken = default)
        //{
        //    return await _context.Meetings
        //        .Include(m => m.AgendaItems)
        //        .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        //}
        public async Task<Meeting?> GetWithAgendaAsync(MeetingId id, CancellationToken cancellationToken = default)
        {
            return await _context.Meetings
                .Include(m => m.AgendaItems)
                .AsSplitQuery()
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<Meeting?> GetWithAgendaAndAttachmentsAsync(MeetingId id, CancellationToken cancellationToken = default)
        {
            return await _context.Meetings
                .Include(m => m.AgendaItems)          // المستوى 1: البنود
                    .ThenInclude(ai => ai.Attachments) // المستوى 2: المرفقات (هنا يكمن الحل)
                .AsSplitQuery() // مهم جداً هنا لأن البيانات قد تكون كبيرة
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        //public async Task<Meeting?> GetWithAttendeesAsync(MeetingId id, CancellationToken cancellationToken = default)
        //{
        //    return await _context.Meetings
        //        .Include(m => m.Attendances)
        //        .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        //}
        public async Task<Meeting?> GetWithAttendeesAsync(MeetingId id, CancellationToken ct)
        {
            return await _context.Meetings
                .Include(m => m.Attendances) // ✅ ضروري جداً
                .AsSplitQuery()
                .FirstOrDefaultAsync(m => m.Id == id, ct);
        }

        public async Task<Meeting?> GetWithMinutesAsync(MeetingId id, CancellationToken cancellationToken = default)
        {
            return await _context.Meetings
                .Include(m => m.Minutes)
                    .ThenInclude(mom => mom!.Versions)
                .Include(m => m.Minutes)
                    .ThenInclude(mom => mom!.Attachments)
                .Include(m => m.Minutes)
                    .ThenInclude(mom => mom!.Decisions)
                .Include(m => m.Minutes)
                    .ThenInclude(mom => mom!.ActionItems)
                .AsSplitQuery() // ضروري للأداء عند جلب كل هذه التفاصيل
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<Meeting?> GetForSchedulingAsync(MeetingId id, CancellationToken cancellationToken = default)
        {
            return await _context.Meetings
                .Include(m => m.AgendaItems)  // ضروري للتحقق من الشرط (Invariant)
                .Include(m => m.Attendances)  // ضروري لملء بيانات الحدث
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public IQueryable<Meeting> GetTableNoTracking()
        {
            return _context.Meetings.AsNoTracking();
        }


        public async Task<bool> HasConflictAsync(
            DateTime startDate,
            DateTime endDate,
            string roomName,
            MeetingId? excludeMeetingId = null,
            CancellationToken cancellationToken = default)
        {
            // التحقق فقط للاجتماعات غير الملغاة والمنعقدة في نفس الغرفة
            var query = _context.Meetings
                .AsNoTracking() // لأداء أسرع (Read-Only)
                .Where(m => m.Status != MeetingStatus.Cancelled)
                .Where(m => m.Location.Type == LocationType.Physical || m.Location.Type == LocationType.Hybrid)
                .Where(m => m.Location.RoomName == roomName);

            // استثناء الاجتماع الحالي (مهم جداً في حالة التعديل Update)
            if (excludeMeetingId != null)
            {
                query = query.Where(m => m.Id != excludeMeetingId);
            }

            // شرط التقاطع الزمني (Overlap Condition)
            // (StartA < EndB) and (EndA > StartB)
            return await query.AnyAsync(m => m.StartDate < endDate && m.EndDate > startDate, cancellationToken);
        }
    }
}
