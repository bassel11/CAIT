using FluentValidation;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingCore.Enums;

namespace MeetingApplication.Features.Meetings.Commands.Validators
{
    public class CreateMeetingCommandValidator : AbstractValidator<CreateMeetingCommand>
    {
        public CreateMeetingCommandValidator()
        {
            // 1. التحقق من العناوين والتواريخ الأساسية
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.");

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate).WithMessage("StartDate must be before EndDate.");

            // 2. الحالة الأولى: إذا كان الاجتماع متكرراً (IsRecurring = true)
            // يجب أن يكون النوع صحيحاً (Weekly, Monthly...) وليس None أو فارغاً
            RuleFor(x => x.RecurrenceType)
                .Must(BeAValidRecurrenceType)
                .WithMessage("RecurrenceType must be one of: Weekly, Monthly, Quarterly, Yearly.")
                .When(x => x.IsRecurring);

            // 3. الحالة الثانية: إذا كان الاجتماع غير متكرر (IsRecurring = false)
            // يجب أن يكون النوع فارغاً أو "None" فقط.
            // هذا يمنع تمرير قيم عشوائية مثل "ttt" التي تسبب انهيار AutoMapper
            RuleFor(x => x.RecurrenceType)
                .Must(BeEmptyOrNone)
                .WithMessage("RecurrenceType must be empty or 'None' when the meeting is not recurring.")
                .When(x => !x.IsRecurring);
        }

        // دالة التحقق للاجتماعات المتكررة
        private bool BeAValidRecurrenceType(string? recurrenceType)
        {
            if (string.IsNullOrWhiteSpace(recurrenceType)) return false;

            // يحاول تحويل النص إلى Enum (تجاهل حالة الأحرف)
            bool isParsed = Enum.TryParse<RecurrenceType>(recurrenceType, true, out var result);

            // التأكد من:
            // 1. التحويل نجح
            // 2. القيمة موجودة فعلاً في الـ Enum (لمنع الأرقام العشوائية)
            // 3. القيمة ليست None (لأن المتكرر يحتاج فترة زمنية)
            return isParsed && Enum.IsDefined(typeof(RecurrenceType), result) && result != RecurrenceType.None;
        }

        // دالة التحقق للاجتماعات غير المتكررة
        private bool BeEmptyOrNone(string? recurrenceType)
        {
            // نسمح بالقيمة الفارغة
            if (string.IsNullOrWhiteSpace(recurrenceType)) return true;

            // نسمح بكلمة "None" بأي شكل (none, NONE, None)
            if (recurrenceType.Equals("None", StringComparison.OrdinalIgnoreCase)) return true;

            // أي نص آخر يعتبر خطأ
            return false;
        }
    }
}