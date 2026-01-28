using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.AgendaTemplates.Commands.Models
{
    public record UpdateAgendaTemplateCommand : ICommand<Result>
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public string Description { get; init; } = default!;

        // سنقوم باستبدال القائمة كاملة للتبسيط (Full Replacement)
        // أو يمكن تعقيدها (Add/Update/Remove)، لكن في القوالب الاستبدال مقبول
        public List<TemplateItemDto> Items { get; init; } = new();
    }
}
