using MediatR;
using MeetingApplication.Features.AgendaItems.Queries.Results;

namespace MeetingApplication.Features.AgendaItems.Queries.Models
{
    public class GetAgendaItemsQuery : IRequest<List<GetAgendaItemResponse>>
    {
        public Guid MeetingId { get; set; }
    }
}
