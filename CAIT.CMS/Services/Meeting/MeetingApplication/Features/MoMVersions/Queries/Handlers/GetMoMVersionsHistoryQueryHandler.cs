using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Data;
using MeetingApplication.Features.MoMVersions.Queries.Models;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMVersions.Queries.Handlers
{
    public class GetMoMVersionsHistoryQueryHandler : IQueryHandler<GetMoMVersionsHistoryQuery, Result<List<MoMVersionSummaryDto>>>
    {
        private readonly IMeetingDbContext _context;

        public GetMoMVersionsHistoryQueryHandler(IMeetingDbContext context) => _context = context;

        public async Task<Result<List<MoMVersionSummaryDto>>> Handle(GetMoMVersionsHistoryQuery req, CancellationToken ct)
        {
            var meetingId = MeetingId.Of(req.MeetingId);

            var history = await _context.Minutes
                .AsNoTracking()
                .Where(m => m.MeetingId == meetingId)
                .SelectMany(m => m.Versions)
                .OrderByDescending(v => v.VersionNumber)
                .Select(v => new MoMVersionSummaryDto
                {
                    VersionNumber = v.VersionNumber,
                    ModifiedAt = v.ModifiedAt,
                    ModifiedBy = v.ModifiedBy.Value
                })
                .ToListAsync(ct);

            return Result<List<MoMVersionSummaryDto>>.Success(history);
        }
    }
}
