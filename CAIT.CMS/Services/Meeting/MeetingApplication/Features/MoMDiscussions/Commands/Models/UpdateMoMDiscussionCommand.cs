using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMDiscussions.Commands.Models
{
    public record UpdateMoMDiscussionCommand(Guid MeetingId, Guid TopicId, string Content)
        : ICommand<Result>;
}
