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
