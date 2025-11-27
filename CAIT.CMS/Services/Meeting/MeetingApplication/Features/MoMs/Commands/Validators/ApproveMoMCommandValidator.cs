using FluentValidation;
using MeetingApplication.Features.MoMs.Commands.Models;

namespace MeetingApplication.Features.MoMs.Commands.Validators
{
    public class ApproveMoMCommandValidator : AbstractValidator<ApproveMoMCommand>
    {
        public ApproveMoMCommandValidator() => RuleFor(x => x.MoMId).NotEmpty();
    }
}
