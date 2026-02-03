using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Data;
using MeetingApplication.Features.MoMs.Queries.Models;
using MeetingApplication.Features.MoMs.Queries.Results;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMs.Queries.Handlers
{
    public class GetMoMByMeetingIdQueryHandler
        : IQueryHandler<GetMoMByMeetingIdQuery, Result<MoMResponse>>
    {
        private readonly IMeetingDbContext _dbContext;

        public GetMoMByMeetingIdQueryHandler(IMeetingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<MoMResponse>> Handle(GetMoMByMeetingIdQuery req, CancellationToken ct)
        {
            var meetingId = MeetingId.Of(req.MeetingId);

            var mom = await _dbContext.Minutes
                .AsNoTracking()
                .Where(m => m.MeetingId == meetingId)
                .Select(m => new MoMResponse
                {
                    Id = m.Id.Value,
                    MeetingId = m.MeetingId.Value,
                    Status = m.Status.ToString(),
                    ContentHtml = m.FullContentHtml,
                    Version = m.VersionNumber,
                    CreatedAt = m.CreatedAt,
                    LastModified = m.LastTimeModified ?? m.CreatedAt,
                    ApprovedBy = m.ApprovedBy,
                    // ✅ 1. قائمة الحضور (للعرض في الجدول)
                    AttendanceList = m.AttendanceSnapshot.Select(a => new MoMAttendanceDto
                    {
                        Id = a.Id.Value,
                        MemberName = a.MemberName,
                        Role = a.Role,
                        IsPresent = a.IsPresent,
                        Status = a.Status.ToString(),
                        Notes = a.Notes ?? a.AbsenceReason
                    }).ToList(),

                    // ✅ 2. قائمة النقاشات (للعرض في المحرر)
                    Discussions = m.Discussions.Select(d => new MoMDiscussionDto
                    {
                        Id = d.Id.Value,
                        TopicTitle = d.TopicTitle,
                        Content = d.DiscussionContent,
                        OriginalAgendaItemId = d.OriginalAgendaItemId != null ? d.OriginalAgendaItemId.Value : null
                    }).OrderBy(d => d.OriginalAgendaItemId).ToList(), // ترتيب حسب الأجندة

                    // ✅ 3. القرارات والمهام
                    Decisions = m.Decisions.Select(d => new DecisionDto(d.Id.Value, d.Title, d.Text)).ToList(),
                    ActionItems = m.ActionItems.Select(t => new ActionItemDto(t.Id.Value, t.TaskTitle, t.AssigneeId)).ToList(),

                    DecisionsCount = m.Decisions.Count,
                    ActionItemsCount = m.ActionItems.Count,
                    AttachmentsCount = m.Attachments.Count
                })
                .FirstOrDefaultAsync(ct);

            if (mom == null)
                return Result<MoMResponse>.Failure("Minutes not found for this meeting.");

            return Result<MoMResponse>.Success(mom);
        }
    }
}
