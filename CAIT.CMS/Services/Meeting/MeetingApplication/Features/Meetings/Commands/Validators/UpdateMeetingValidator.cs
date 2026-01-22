using FluentValidation;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingCore.Enums.MeetingEnums;

namespace MeetingApplication.Features.Meetings.Commands.Validators
{
    public class UpdateMeetingValidator : AbstractValidator<UpdateMeetingCommand>
    {
        public UpdateMeetingValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200);

            // التحقق من صحة بيانات الموقع المعدلة
            RuleFor(x => x.LocationType).IsInEnum();

            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.LocationRoom) || !string.IsNullOrWhiteSpace(x.LocationAddress))
                .When(x => x.LocationType == (int)LocationType.Physical || x.LocationType == (int)LocationType.Hybrid)
                .WithMessage("Physical meetings require a Room Name or Address.");

            RuleFor(x => x.LocationOnlineUrl)
                .NotEmpty()
                .When(x => x.LocationType == (int)LocationType.Online || x.LocationType == (int)LocationType.Hybrid)
                .WithMessage("Online URL is required.");

        }
    }
}
