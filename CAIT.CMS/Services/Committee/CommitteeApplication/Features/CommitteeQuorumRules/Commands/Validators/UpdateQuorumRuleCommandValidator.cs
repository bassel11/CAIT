using CommitteeApplication.Features.CommitteeQuorumRules.Commands.Models;
using CommitteeCore.Enums;
using FluentValidation;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Commands.Validators
{
    public class UpdateQuorumRuleCommandValidator : AbstractValidator<UpdateQuorumRuleCommand>
    {
        public UpdateQuorumRuleCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Type).IsInEnum();

            // 1. قواعد النسبة المئوية
            When(x => x.Type == QuorumType.Percentage || x.Type == QuorumType.PercentagePlusOne, () =>
            {
                RuleFor(x => x.ThresholdPercent)
                    .NotNull().WithMessage("Threshold Percent is required.")
                    .InclusiveBetween(1, 100);

                // تأكد من أن المستخدم لم يرسل عدداً ثابتاً بالخطأ
                RuleFor(x => x.AbsoluteCount)
                    .Null().WithMessage("Absolute Count must be null for percentage types.");
            });

            // 2. قواعد العدد الثابت
            When(x => x.Type == QuorumType.AbsoluteNumber, () =>
            {
                RuleFor(x => x.AbsoluteCount)
                    .NotNull().WithMessage("Absolute Count is required.")
                    .GreaterThan(0);

                // تأكد من أن المستخدم لم يرسل نسبة مئوية بالخطأ
                RuleFor(x => x.ThresholdPercent)
                    .Null().WithMessage("Threshold Percent must be null for absolute number types.");
            });
        }
    }
}
