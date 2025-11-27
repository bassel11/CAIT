using MediatR;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public class SubmitMoMForApprovalCommand : IRequest<Unit>
    {
        public Guid MoMId { get; set; }
    }
}
