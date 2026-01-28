using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.AgendaItems.Commands.Models
{
    public record LoadAgendaTemplateCommand : ICommand<Result>
    {
        public Guid MeetingId { get; init; }
        public Guid TemplateId { get; init; } // ✅ نستخدم ID بدلاً من الاسم للدقة
    }
}
