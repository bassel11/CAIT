using MediatR;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public class PublishMoMCommand : IRequest<string>
    {
        public Guid MoMId { get; set; }
    }
}
