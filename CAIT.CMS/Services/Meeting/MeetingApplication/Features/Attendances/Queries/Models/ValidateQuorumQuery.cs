using MediatR;
using MeetingApplication.Features.Attendances.Queries.Results;

namespace MeetingApplication.Features.Attendances.Queries.Models
{
    public class ValidateQuorumQuery : IRequest<QuorumValidationResult>
    {
        public Guid MeetingId { get; }
    }
}
