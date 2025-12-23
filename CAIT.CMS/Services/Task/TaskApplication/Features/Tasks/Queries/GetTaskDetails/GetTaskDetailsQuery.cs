using BuildingBlocks.Shared.CQRS;
using TaskApplication.Dtos;

namespace TaskApplication.Features.Tasks.Queries.GetTaskDetails
{
    public record GetTaskDetailsQuery(Guid TaskId) : IQuery<TaskDto>;
}
