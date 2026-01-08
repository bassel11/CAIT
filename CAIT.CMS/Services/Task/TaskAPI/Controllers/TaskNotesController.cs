using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApplication.Features.Notes.Commands.AddTaskNote;
using TaskApplication.Features.Notes.Commands.EditTaskNote;
using TaskApplication.Features.Notes.Commands.RemoveTaskNote;
using TaskApplication.Features.Notes.Queries.GetNotes;

namespace TaskAPI.Controllers
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/TaskNotes")]

    //[Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class TaskNotesController : BaseApiController
    {
        private readonly IMediator _mediator;

        public TaskNotesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{id}/notes")]
        [Authorize(Policy = "Permission:TaskNote.View")]
        public async Task<IActionResult> GetNotes(Guid id)
        {
            var result = await _mediator.Send(new GetTaskNotesQuery(id));
            return Ok(result);
        }

        // 2. Add Note
        [HttpPost("{id}/notes")]
        [Authorize(Policy = "Permission:TaskNote.Create")]
        public async Task<IActionResult> AddNote(Guid id, [FromBody] AddNoteRequest request)
        {
            var command = new AddTaskNoteCommand(id, request.Content);
            var noteId = await _mediator.Send(command);
            return Ok(new { NoteId = noteId });
        }

        // 3. Edit Note
        [HttpPut("{id}/notes/{noteId}")]
        [Authorize(Policy = "Permission:TaskNote.Update")]
        public async Task<IActionResult> EditNote(Guid id, Guid noteId, [FromBody] EditNoteRequest request)
        {
            var command = new EditTaskNoteCommand(id, noteId, request.Content);
            await _mediator.Send(command);
            return NoContent();
        }

        // 4. Delete Note
        [HttpDelete("{id}/notes/{noteId}")]
        [Authorize(Policy = "Permission:TaskNote.Delete")]
        public async Task<IActionResult> DeleteNote(Guid id, Guid noteId)
        {
            var command = new RemoveTaskNoteCommand(id, noteId);
            await _mediator.Send(command);
            return NoContent();
        }

    }
}
