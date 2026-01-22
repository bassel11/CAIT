using FluentValidation;
using MeetingApplication.Features.MoMs.Commands.Models;

namespace MeetingApplication.Features.MoMs.Commands.Validators
{
    public class PublishMoMCommandValidator : AbstractValidator<PublishMoMCommand>
    {
        public PublishMoMCommandValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
        }
    }
}
