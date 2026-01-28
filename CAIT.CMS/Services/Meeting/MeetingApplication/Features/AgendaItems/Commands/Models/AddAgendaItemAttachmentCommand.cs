using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.AgendaItems.Commands.Models
{
    public record AddAgendaItemAttachmentCommand : ICommand<Result<Guid>>
    {
        public Guid MeetingId { get; init; }
        public Guid AgendaItemId { get; init; }

        // بيانات الملف (عادة تأتي بعد الرفع على Storage Service والحصول على الرابط)
        public string FileName { get; init; } = default!;
        public string FileUrl { get; init; } = default!;
        public string ContentType { get; init; } = default!;
    }
}
