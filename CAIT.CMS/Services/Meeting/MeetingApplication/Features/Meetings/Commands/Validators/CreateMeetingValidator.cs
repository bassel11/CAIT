using FluentValidation;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingCore.Enums.MeetingEnums;

namespace MeetingApplication.Features.Meetings.Commands.Validators
{
    public class CreateMeetingCommandValidator : AbstractValidator<CreateMeetingCommand>
    {
        public CreateMeetingCommandValidator()
        {
            // 1. الأساسيات
            RuleFor(x => x.CommitteeId)
                .NotEmpty().WithMessage("CommitteeId is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            // 2. التواريخ
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required.");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .GreaterThan(x => x.StartDate).WithMessage("EndDate must be after StartDate.");

            RuleFor(x => x.TimeZone)
                .NotEmpty().WithMessage("TimeZone is required.");

            // 3. الموقع (منطق شرطي)
            RuleFor(x => x.LocationType)
                .IsInEnum().WithMessage("Invalid LocationType.");

            // إذا كان حضوري أو هجين، يجب وجود غرفة أو عنوان
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.LocationRoom) || !string.IsNullOrWhiteSpace(x.LocationAddress))
                .When(x => x.LocationType == LocationType.Physical || x.LocationType == LocationType.Hybrid)
                .WithMessage("Physical meetings require a Room Name or Address.");

            // إذا كان أونلاين أو هجين، يجب وجود رابط
            RuleFor(x => x.LocationOnlineUrl)
                .NotEmpty()
                .When(x => x.LocationType == LocationType.Online || x.LocationType == LocationType.Hybrid)
                .WithMessage("Online URL is required for Online/Hybrid meetings.");

            // 4. التكرار
            RuleFor(x => x.RecurrenceType)
                .IsInEnum().When(x => x.RecurrenceType.HasValue);

            RuleFor(x => x)
                .Must(x => x.RecurrenceType.HasValue && x.RecurrenceType != RecurrenceType.None)
                .When(x => x.IsRecurring && string.IsNullOrEmpty(x.RecurrenceRule))
                .WithMessage("Recurring meetings must have a valid Recurrence Type or Rule.");
        }
    }
}