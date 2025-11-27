using MediatR;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public class CreateMoMCommand : IRequest<Guid>
    {
        public Guid MeetingId { get; set; }
        public string? AttendanceSummary { get; set; }
        public string? AgendaSummary { get; set; }
        public string? DecisionsSummary { get; set; }
        public string? ActionItemsJson { get; set; }
    }
}
