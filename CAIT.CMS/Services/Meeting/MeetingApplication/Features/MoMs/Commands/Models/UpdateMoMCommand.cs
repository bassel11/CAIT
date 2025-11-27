using MediatR;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public class UpdateMoMCommand : IRequest<Guid>
    {
        public Guid MoMId { get; set; }
        public string? AttendanceSummary { get; set; }
        public string? AgendaSummary { get; set; }
        public string? DecisionsSummary { get; set; }
        public string? ActionItemsJson { get; set; }
    }
}
