using MediatR;
using MeetingApplication.Features.Meetings.Commands.Results;
using System.ComponentModel.DataAnnotations;

namespace MeetingApplication.Features.Meetings.Commands.Models
{
    public class UpdateMeetingCommand : IRequest<UpdateMeetingResponse>
    {
        [Required]
        public Guid Id { get; set; }
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
