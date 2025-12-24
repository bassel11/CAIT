using MediatR;

namespace TaskApplication.Features.Automation.Commands.ProcessOverdueTasks
{
    public record ProcessOverdueTasksCommand : IRequest;
}
