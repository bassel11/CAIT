using MediatR;

namespace MeetingApplication.Features.AgendaItems.Commands.Models
{
    public class DeleteAgendaItemCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
