using BuildingBlocks.Shared.CQRS;
using FluentValidation;

namespace TaskApplication.Features.Tasks.Commands.UploadAttachment
{
    public record UploadAttachmentCommand(
         Guid TaskId,
         Stream FileStream,
         string FileName,
         string ContentType,
         long Size
     ) : ICommand<UploadAttachmentResult>;

    public record UploadAttachmentResult(Guid Id);
    public class UploadAttachmentCommandValidator : AbstractValidator<UploadAttachmentCommand>
    {
        public UploadAttachmentCommandValidator()
        {

        }
    }
}
