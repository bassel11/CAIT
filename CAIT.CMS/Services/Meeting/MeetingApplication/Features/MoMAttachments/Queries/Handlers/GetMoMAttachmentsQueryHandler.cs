using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Data;
using MeetingApplication.Features.MoMAttachments.Queries.Models;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMAttachments.Queries.Handlers
{
    public class GetMoMAttachmentsQueryHandler
        : IQueryHandler<GetMoMAttachmentsQuery, Result<List<MoMAttachmentDto>>>
    {
        private readonly IMeetingDbContext _context;

        public GetMoMAttachmentsQueryHandler(IMeetingDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<MoMAttachmentDto>>> Handle(GetMoMAttachmentsQuery req, CancellationToken ct)
        {
            var meetingId = MeetingId.Of(req.MeetingId);

            var items = await _context.Minutes
                .AsNoTracking()
                .Where(m => m.MeetingId == meetingId)
                .SelectMany(m => m.Attachments)
                .OrderByDescending(a => a.UploadedAt)
                .Select(a => new MoMAttachmentDto
                {
                    Id = a.Id.Value,
                    FileName = a.FileName,
                    ContentType = a.ContentType,
                    Size = a.SizeInBytes,
                    UploadedAt = a.UploadedAt,
                    UploadedBy = a.UploadedBy.Value,
                    // مثال لرابط التحميل
                    DownloadUrl = $"/api/v1/meetings/{req.MeetingId}/attachments/{a.Id.Value}/download"
                })
                .ToListAsync(ct);

            return Result<List<MoMAttachmentDto>>.Success(items);
        }
    }
}
