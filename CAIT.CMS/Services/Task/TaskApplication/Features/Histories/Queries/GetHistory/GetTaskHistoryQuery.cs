using BuildingBlocks.Shared.CQRS;
using TaskApplication.Dtos;

namespace TaskApplication.Features.Histories.Queries.GetHistory
{
    public record GetTaskHistoryQuery(Guid TaskId) : IQuery<List<TaskHistoryDto>>;
}
