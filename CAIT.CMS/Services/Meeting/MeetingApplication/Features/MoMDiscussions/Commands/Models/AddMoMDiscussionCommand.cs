using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMDiscussions.Commands.Models
{
    public record AddMoMDiscussionCommand(Guid MeetingId, string Title, string Content)
        : ICommand<Result<Guid>>;
}
