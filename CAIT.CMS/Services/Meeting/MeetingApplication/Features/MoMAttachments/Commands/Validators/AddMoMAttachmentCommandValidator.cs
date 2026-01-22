using FluentValidation;
using MeetingApplication.Features.MoMAttachments.Commands.Models;

namespace MeetingApplication.Features.MoMAttachments.Commands.Validators
{
    public class AddMoMAttachmentCommandValidator
          : AbstractValidator<AddMoMAttachmentCommand>
    {
        public AddMoMAttachmentCommandValidator()
        {
            RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("Filename is required.")
            .MaximumLength(255).WithMessage("Filename is too long.")
            // تحقق بسيط من الامتداد كأمان إضافي
            .Matches(@"^[\w,\s-]+\.[A-Za-z0-9]+$").WithMessage("Invalid filename format.");

            RuleFor(x => x.ContentType)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.SizeInBytes)
                .GreaterThan(0).WithMessage("File cannot be empty.")
                .LessThanOrEqualTo(50 * 1024 * 1024).WithMessage("File size exceeds 50MB limit."); // مثال

            RuleFor(x => x.StoragePath).NotEmpty();
        }
    }
}
