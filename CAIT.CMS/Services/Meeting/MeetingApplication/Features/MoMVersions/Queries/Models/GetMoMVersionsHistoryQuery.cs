using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMVersions.Queries.Models
{
    // DTO List (خفيف بدون محتوى)
    public class MoMVersionSummaryDto
    {
        public int VersionNumber { get; set; }
        public DateTime ModifiedAt { get; set; }
        public Guid ModifiedBy { get; set; }
    }

    // Query List
    public record GetMoMVersionsHistoryQuery(Guid MeetingId)
        : IQuery<Result<List<MoMVersionSummaryDto>>>;
}
