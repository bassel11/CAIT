using MediatR;

namespace MeetingApplication.Features.AgendaItems.Commands.Models
{
    public class AddAgendaItemCommand : IRequest<Guid>
    {
        public Guid MeetingId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }
    }
}
