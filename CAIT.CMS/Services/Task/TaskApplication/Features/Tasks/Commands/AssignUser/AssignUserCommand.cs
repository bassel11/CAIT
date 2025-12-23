using BuildingBlocks.Shared.CQRS;
using FluentValidation;

namespace TaskApplication.Features.Tasks.Commands.AssignUser
{
    public record AssignUserCommand(Guid TaskId, Guid UserId, string Email, string Name)
        : ICommand<AssignUserResult>;

    public record AssignUserResult(Guid Id);

    public class AssignUserCommandValidator : AbstractValidator<AssignUserCommand>
    {
        public AssignUserCommandValidator()
        {

        }
    }
}
