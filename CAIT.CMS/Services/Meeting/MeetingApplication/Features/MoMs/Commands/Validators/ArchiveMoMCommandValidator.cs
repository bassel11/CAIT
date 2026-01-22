using FluentValidation;
using MeetingApplication.Features.MoMs.Commands.Models;

namespace MeetingApplication.Features.MoMs.Commands.Validators
{
    public class ArchiveMoMCommandValidator : AbstractValidator<ArchiveMoMCommand>
    {
        public ArchiveMoMCommandValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
        }
    }
}
