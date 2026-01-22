using FluentValidation;
using MeetingApplication.Features.Meetings.Commands.Models;

namespace MeetingApplication.Features.Meetings.Commands.Validators
{
    public class CompleteMeetingCommandValidator : AbstractValidator<CompleteMeetingCommand>
    {
        public CompleteMeetingCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
