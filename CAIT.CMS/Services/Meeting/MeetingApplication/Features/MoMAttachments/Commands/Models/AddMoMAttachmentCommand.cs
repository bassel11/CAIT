using MediatR;

namespace MeetingApplication.Features.MoMAttachments.Commands.Models
{
    public class AddMoMAttachmentCommand : IRequest<Guid>
    {
        public Guid MoMId { get; set; }
        public byte[] Content { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
