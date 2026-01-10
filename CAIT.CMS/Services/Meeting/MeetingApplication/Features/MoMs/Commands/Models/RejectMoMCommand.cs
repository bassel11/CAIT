using MediatR;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public record RejectMoMCommand(Guid Id, string Reason) : IRequest<Guid>;
}
