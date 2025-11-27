using MediatR;
using MeetingApplication.Features.MoMs.Queries.Results;

namespace MeetingApplication.Features.MoMs.Queries.Models
{
    public class GetMoMByIdQuery : IRequest<GetMinutesResponse?>
    {
        public Guid MoMId { get; set; }
    }
}
