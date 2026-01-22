using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMVersions.Queries.Models
{
    // DTO Detail (المحتوى الكامل)
    public class MoMVersionDetailDto
    {
        public int VersionNumber { get; set; }
        public string ContentHtml { get; set; } = default!;
        public DateTime ModifiedAt { get; set; }
        public Guid ModifiedBy { get; set; }
    }

    // Query
    public record GetMoMVersionDetailQuery(Guid MeetingId, int VersionNumber)
        : IQuery<Result<MoMVersionDetailDto>>;
}
