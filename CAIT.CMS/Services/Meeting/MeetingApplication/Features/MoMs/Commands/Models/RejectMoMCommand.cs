using MediatR;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public class RejectMoMCommand : IRequest<Unit>
    {
        public Guid MoMId { get; set; }
        public string Reason { get; set; }
    }
}
