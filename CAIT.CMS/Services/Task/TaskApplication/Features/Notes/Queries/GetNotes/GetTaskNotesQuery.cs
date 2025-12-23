using BuildingBlocks.Shared.CQRS;
using TaskApplication.Dtos;

namespace TaskApplication.Features.Notes.Queries.GetNotes
{
    public record GetTaskNotesQuery(Guid TaskId) : IQuery<List<TaskNoteDto>>;
}
