using BuildingBlocks.Shared.CQRS;
using MeetingApplication.Features.Attendances.Queries.Results;

namespace MeetingApplication.Features.Attendances.Queries.Models
{
    public class ValidateQuorumQuery : IQuery<QuorumValidationResult>
    {
        public Guid MeetingId { get; set; }
        public ValidateQuorumQuery(Guid meetingId)
        {
            MeetingId = meetingId;
        }
    }
}
