using BuildingBlocks.Shared.CQRS;

namespace TaskApplication.Features.Tasks.Commands.UnassignUser
{
    public record UnassignUserCommand(Guid TaskId, Guid UserId)
        : ICommand<UnassignUserResult>;
    public record UnassignUserResult(Guid UserId);
}
