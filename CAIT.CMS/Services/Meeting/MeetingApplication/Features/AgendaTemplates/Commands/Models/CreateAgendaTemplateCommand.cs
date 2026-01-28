using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.AgendaTemplates.Commands.Models
{
    public record CreateAgendaTemplateCommand : ICommand<Result<Guid>>
    {
        public string Name { get; init; } = default!;
        public string Description { get; init; } = default!;
        public List<TemplateItemDto> Items { get; init; } = new();
    }

    public record TemplateItemDto(string Title, int DurationMinutes, string? Description, int SortOrder);
}
