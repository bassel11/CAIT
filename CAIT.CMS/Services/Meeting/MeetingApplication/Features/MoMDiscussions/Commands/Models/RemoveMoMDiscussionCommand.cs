using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;

namespace MeetingApplication.Features.MoMDiscussions.Commands.Models
{
    public record RemoveMoMDiscussionCommand(Guid MeetingId, Guid TopicId)
        : ICommand<Result>;
}
