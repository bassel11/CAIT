using FluentValidation;
using MeetingApplication.Features.MoMAttachments.Commands.Models;

namespace MeetingApplication.Features.MoMAttachments.Commands.Validators
{
    public class AddMoMAttachmentCommandValidator
          : AbstractValidator<AddMoMAttachmentCommand>
    {
        public AddMoMAttachmentCommandValidator()
        {
            RuleFor(x => x.MoMId).NotEmpty();
            RuleFor(x => x.Content).NotEmpty();
            RuleFor(x => x.FileName).NotEmpty().MaximumLength(300);
            RuleFor(x => x.ContentType).NotEmpty().MaximumLength(200);
        }
    }
}
