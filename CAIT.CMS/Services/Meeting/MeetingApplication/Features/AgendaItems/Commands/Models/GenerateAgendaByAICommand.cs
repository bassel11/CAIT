using MediatR;
using MeetingApplication.Features.AgendaItems.Queries.Results;

namespace MeetingApplication.Features.AgendaItems.Commands.Models
{
    public class GenerateAgendaByAICommand : IRequest<List<GetAgendaItemResponse>>
    {
        public Guid MeetingId { get; set; }
        public string Purpose { get; set; }
    }
}
