using MediatR;

namespace MeetingApplication.Features.AgendaItems.Commands.Models
{
    public class UpdateAgendaItemCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }
    }
}
