using FluentValidation;
using MeetingApplication.Features.MoMs.Commands.Models;

namespace MeetingApplication.Features.MoMs.Commands.Validators
{
    public class CreateMoMCommandValidator : AbstractValidator<CreateMoMCommand>
    {
        public CreateMoMCommandValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            RuleFor(x => x.AttendanceSummary).MaximumLength(4000);
            RuleFor(x => x.AgendaSummary).MaximumLength(4000);
            RuleFor(x => x.DecisionsSummary).MaximumLength(4000);
            RuleFor(x => x.ActionItemsJson).MaximumLength(4000);
        }
    }
}
