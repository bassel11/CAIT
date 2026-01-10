using MediatR;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public record SubmitMoMForApprovalCommand(Guid MoMId) : IRequest<Unit>;
}
