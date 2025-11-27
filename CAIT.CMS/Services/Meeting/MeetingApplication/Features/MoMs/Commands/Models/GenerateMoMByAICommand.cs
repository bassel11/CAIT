using MediatR;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public class GenerateMoMByAICommand : IRequest<Guid> // returns MoMId
    {
        public Guid MeetingId { get; set; }
        public bool FromTranscript { get; set; } = true;
    }
}
