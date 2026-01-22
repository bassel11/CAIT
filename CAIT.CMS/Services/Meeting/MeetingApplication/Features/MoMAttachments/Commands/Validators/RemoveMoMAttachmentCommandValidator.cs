using FluentValidation;
using MeetingApplication.Features.MoMAttachments.Commands.Models;

namespace MeetingApplication.Features.MoMAttachments.Commands.Validators
{
    public class RemoveMoMAttachmentCommandValidator : AbstractValidator<RemoveMoMAttachmentCommand>
    {
        public RemoveMoMAttachmentCommandValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            RuleFor(x => x.AttachmentId).NotEmpty();
        }
    }
}
