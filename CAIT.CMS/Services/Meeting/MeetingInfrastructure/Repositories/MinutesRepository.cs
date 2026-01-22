using BuildingBlocks.Shared.Abstractions;
using MeetingCore.Entities;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;
using MeetingInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeetingInfrastructure.Repositories
{
    public class MinutesRepository : IMinutesRepository
    {
        private readonly MeetingDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public MinutesRepository(MeetingDbContext context)
        {
            _context = context;
            _unitOfWork = new UnitOfWork(context);
        }

        public IUnitOfWork UnitOfWork => _unitOfWork;

        // ---------------------------------------------------------
        // 1. Add
        // ---------------------------------------------------------
        public async Task AddAsync(MinutesOfMeeting mom, CancellationToken ct = default)
        {
            await _context.Minutes.AddAsync(mom, ct);
        }

        // ---------------------------------------------------------
        // 2. Simple Get (الأخف والأسرع)
        // ---------------------------------------------------------
        public async Task<MinutesOfMeeting?> GetByMeetingIdSimpleAsync(MeetingId meetingId, CancellationToken ct = default)
        {
            // لا يوجد أي Include. سريع جداً.
            // يجلب فقط الجدول الرئيسي (MinutesOfMeetings)
            return await _context.Minutes
                .FirstOrDefaultAsync(m => m.MeetingId == meetingId, ct);
        }

        // ---------------------------------------------------------
        // 3. Decisions Context
        // ---------------------------------------------------------
        public async Task<MinutesOfMeeting?> GetWithDecisionsByMeetingIdAsync(MeetingId meetingId, CancellationToken ct = default)
        {
            return await _context.Minutes
                .Include(m => m.Decisions) // ✅ نحمل القرارات فقط
                .FirstOrDefaultAsync(m => m.MeetingId == meetingId, ct);
        }

        // ---------------------------------------------------------
        // 4. Action Items Context
        // ---------------------------------------------------------
        public async Task<MinutesOfMeeting?> GetWithActionItemsByMeetingIdAsync(MeetingId meetingId, CancellationToken ct = default)
        {
            return await _context.Minutes
                .Include(m => m.ActionItems) // ✅ نحمل المهام فقط
                .FirstOrDefaultAsync(m => m.MeetingId == meetingId, ct);
        }

        // ---------------------------------------------------------
        // 5. Attachments Context
        // ---------------------------------------------------------
        public async Task<MinutesOfMeeting?> GetWithAttachmentsByMeetingIdAsync(MeetingId meetingId, CancellationToken ct = default)
        {
            return await _context.Minutes
                .Include(m => m.Attachments) // ✅ نحمل المرفقات فقط
                .FirstOrDefaultAsync(m => m.MeetingId == meetingId, ct);
        }

        // ---------------------------------------------------------
        // 6. Versions Context
        // ---------------------------------------------------------
        public async Task<MinutesOfMeeting?> GetWithVersionsByMeetingIdAsync(MeetingId meetingId, CancellationToken ct = default)
        {
            return await _context.Minutes
                .Include(m => m.Versions) // ✅ نحمل الأرشيف فقط
                .FirstOrDefaultAsync(m => m.MeetingId == meetingId, ct);
        }

        // ---------------------------------------------------------
        // 7. Full Graph (الأثقل - يستخدم بحذر)
        // ---------------------------------------------------------
        public async Task<MinutesOfMeeting?> GetFullGraphByMeetingIdAsync(MeetingId meetingId, CancellationToken ct = default)
        {
            return await _context.Minutes
                .Include(m => m.Decisions)
                .Include(m => m.ActionItems)
                // .Include(m => m.Attachments) // المرفقات غالباً لا نحتاجها في حدث الاعتماد (حسب البيزنس)
                // .Include(m => m.Versions)    // النسخ القديمة لا تهمنا عند الاعتماد

                .AsSplitQuery() // 🚀 هام جداً: يفصل الاستعلامات لمنع تضخم البيانات (Cartesian Explosion)
                .FirstOrDefaultAsync(m => m.MeetingId == meetingId, ct);
        }

        // ---------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------
        public async Task<bool> ExistsForMeetingAsync(MeetingId meetingId, CancellationToken ct = default)
        {
            return await _context.Minutes.AnyAsync(m => m.MeetingId == meetingId, ct);
        }
    }
}