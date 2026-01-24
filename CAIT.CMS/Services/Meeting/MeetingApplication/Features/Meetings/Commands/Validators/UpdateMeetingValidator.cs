using FluentValidation;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingCore.Enums.MeetingEnums;

public class UpdateMeetingValidator : AbstractValidator<UpdateMeetingCommand>
{
    public UpdateMeetingValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);

        // ✅ التحقق أسهل الآن
        RuleFor(x => x.LocationType).IsInEnum();

        // الشرط للمكان الفيزيائي
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.LocationRoom) || !string.IsNullOrWhiteSpace(x.LocationAddress))
            .When(x => x.LocationType == LocationType.Physical || x.LocationType == LocationType.Hybrid)
            .WithMessage("Physical meetings require a Room Name or Address.");

        // الشرط للمكان الأونلاين
        RuleFor(x => x.LocationOnlineUrl)
            .NotEmpty()
            .When(x => x.LocationType == LocationType.Online || x.LocationType == LocationType.Hybrid)
            .WithMessage("Online URL is required.");
    }
}