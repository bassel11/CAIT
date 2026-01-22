using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMs.Commands.Models
{
    public record GenerateMoMByAICommand(Guid MeetingId, string Transcript)
        : ICommand<Result<string>>;
}
