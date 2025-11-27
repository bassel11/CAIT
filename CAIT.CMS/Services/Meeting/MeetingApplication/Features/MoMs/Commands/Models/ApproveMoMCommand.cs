using MediatR;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public class ApproveMoMCommand : IRequest<Unit>
    {
        public Guid MoMId { get; set; }
    }
}
