using FluentValidation;
using MeetingApplication.Features.MoMs.Commands.Models;

namespace MeetingApplication.Features.MoMs.Commands.Validators
{
    public class GenerateMoMByAICommandValidator : AbstractValidator<GenerateMoMByAICommand>
    {
        public GenerateMoMByAICommandValidator() => RuleFor(x => x.MeetingId).NotEmpty();
    }
}
