using MediatR;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public class ArchiveMoMCommand : IRequest<Unit>
    {
        public Guid MoMId { get; set; }
    }
}
