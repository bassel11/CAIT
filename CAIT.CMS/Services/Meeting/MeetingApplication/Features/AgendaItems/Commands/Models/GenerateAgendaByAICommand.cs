using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.AgendaItems.Commands.Models
{
    public record GenerateAgendaByAICommand(
        Guid MeetingId,
        string MeetingPurpose // مثال: "مراجعة مشاريع الربع الأول"
    ) : ICommand<Result<List<string>>>;
}
