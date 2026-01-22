using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MediatR;
using MeetingApplication.Data;
using MeetingApplication.Features.MoMVersions.Queries.Models;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMVersions.Queries.Handlers
{
    public class GetMoMVersionDetailQueryHandler
        : IQueryHandler<GetMoMVersionDetailQuery, Result<MoMVersionDetailDto>>
    {
        private readonly IMeetingDbContext _context;

        public GetMoMVersionDetailQueryHandler(IMeetingDbContext context) => _context = context;

        public async Task<Result<MoMVersionDetailDto>> Handle(GetMoMVersionDetailQuery req, CancellationToken ct)
        {
            var meetingId = MeetingId.Of(req.MeetingId);

            var version = await _context.Minutes
                .AsNoTracking()
                .Where(m => m.MeetingId == meetingId)
                .SelectMany(m => m.Versions)
                .Where(v => v.VersionNumber == req.VersionNumber)
                .Select(v => new MoMVersionDetailDto
                {
                    VersionNumber = v.VersionNumber,
                    ContentHtml = v.Content, // هنا نجلب المحتوى الثقيل
                    ModifiedAt = v.ModifiedAt,
                    ModifiedBy = v.ModifiedBy.Value
                })
                .FirstOrDefaultAsync(ct);

            if (version == null)
                return Result<MoMVersionDetailDto>.Failure("Version not found.");

            return Result<MoMVersionDetailDto>.Success(version);
        }
    }
}
