using MediatR;
using MeetingApplication.Features.Meetings.Commands.Results;

namespace MeetingApplication.Features.Meetings.Commands.Models
{
    public class CreateMeetingCommand : IRequest<MeetingResponse>
    {
        public Guid CommitteeId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsRecurring { get; set; }
        public string RecurrenceType { get; set; } = "None";
        public Guid CreatedBy { get; set; }
    }
}
