using CommitteeApplication.Features.CommitteeQuorumRules.Commands.Models;
using CommitteeCore.Enums;
using FluentValidation;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Commands.Validators
{
    public class CreateQuorumRuleCommandValidator : AbstractValidator<CreateQuorumRuleCommand>
    {
        public CreateQuorumRuleCommandValidator()
        {
            RuleFor(x => x.CommitteeId).NotEmpty().WithMessage("CommitteeId is required.");
            RuleFor(x => x.Type).IsInEnum().WithMessage("Invalid Quorum Type.");

            // السيناريو 1: التعامل مع النسب المئوية (سواء عادية أو +1)
            When(x => x.Type == QuorumType.Percentage || x.Type == QuorumType.PercentagePlusOne, () =>
            {
                RuleFor(x => x.ThresholdPercent)
                    .NotNull().WithMessage("Threshold Percent is required for percentage types.")
                    .InclusiveBetween(1, 100).WithMessage("Percentage must be between 1 and 100.");

                // قاعدة هامة: التأكد من أن حقل العدد الثابت فارغ لضمان نظافة البيانات
                RuleFor(x => x.AbsoluteCount)
                    .Null().WithMessage("Absolute Count must be null when creating a percentage rule.");
            });

            // السيناريو 2: التعامل مع العدد الثابت
            When(x => x.Type == QuorumType.AbsoluteNumber, () =>
            {
                RuleFor(x => x.AbsoluteCount)
                    .NotNull().WithMessage("Absolute Count is required.")
                    .GreaterThan(0).WithMessage("Absolute Count must be greater than 0.");

                // قاعدة هامة: التأكد من أن حقل النسبة فارغ
                RuleFor(x => x.ThresholdPercent)
                    .Null().WithMessage("Threshold Percent must be null when creating an absolute number rule.");
            });
        }
    }
}
