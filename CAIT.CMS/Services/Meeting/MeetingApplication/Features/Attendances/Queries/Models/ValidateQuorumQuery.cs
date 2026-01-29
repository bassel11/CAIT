using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Attendances.Queries.Results;

namespace MeetingApplication.Features.Attendances.Queries.Models
{
    public class ValidateQuorumQuery : IQuery<Result<QuorumValidationResult>>
    {
        public Guid MeetingId { get; set; }
        public ValidateQuorumQuery(Guid meetingId)
        {
            MeetingId = meetingId;
        }
    }
}
