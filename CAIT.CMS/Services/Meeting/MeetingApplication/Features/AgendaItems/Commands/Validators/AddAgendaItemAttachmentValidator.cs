using FluentValidation;
using MeetingApplication.Features.AgendaItems.Commands.Models;

namespace MeetingApplication.Features.AgendaItems.Commands.Validators
{
    public class AddAgendaItemAttachmentValidator : AbstractValidator<AddAgendaItemAttachmentCommand>
    {
        public AddAgendaItemAttachmentValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            RuleFor(x => x.AgendaItemId).NotEmpty();
            RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.FileUrl).NotEmpty().Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("Invalid File URL.");
            RuleFor(x => x.ContentType).NotEmpty().MaximumLength(100);
        }
    }
}
