using MediatR;
using MeetingApplication.Interfaces;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public class ApproveMoMCommand : IRequest<Unit>, ITransactionalRequest
    {
        public Guid MoMId { get; set; }
        public ApproveMoMCommand(Guid momId)
        {
            MoMId = momId;
        }
    }
}
