using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApplication.Dtos;
using TaskApplication.Features.Notes.Commands.AddTaskNote;
using TaskApplication.Features.Notes.Commands.EditTaskNote;
using TaskApplication.Features.Notes.Commands.RemoveTaskNote;
using TaskApplication.Features.Notes.Queries.GetNotes;

namespace TaskAPI.Controllers
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/tasks/{id}/notes")] // ✅ تحسين المسار ليكون Restful أكثر (notes تابعة لـ tasks)
    [Authorize]
    public class TaskNotesController : BaseApiController
    {
        //private readonly IMediator _mediator;

        //public TaskNotesController(IMediator mediator)
        //{
        //    _mediator = mediator;
        //}

        [HttpGet]
        [Authorize(Policy = "Permission:TaskNote.View")]
        [ProducesResponseType(typeof(Result<List<TaskNoteDto>>), StatusCodes.Status200OK)] // مثال للتوثيق
        public async Task<IActionResult> GetNotes(Guid id)
        {
            var result = await Mediator.Send(new GetTaskNotesQuery(id));
            return Success(result, "DataRetrievedSuccessfully");
        }

        [HttpPost]
        [Authorize(Policy = "Permission:TaskNote.Create")]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddNote(Guid id, [FromBody] AddNoteRequest request)
        {
            var command = new AddTaskNoteCommand(id, request.Content);
            var noteId = await Mediator.Send(command);

            // نرجع NoteId داخل الغلاف الموحد
            return Success(new { NoteId = noteId }, "NoteAddedSuccessfully");
        }

        [HttpPut("{noteId}")]
        [Authorize(Policy = "Permission:TaskNote.Update")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> EditNote(Guid id, Guid noteId, [FromBody] EditNoteRequest request)
        {
            var command = new EditTaskNoteCommand(id, noteId, request.Content);
            await Mediator.Send(command);

            // بدلاً من NoContent، نرجع نجاح برسالة
            return Success("NoteUpdatedSuccessfully");
        }

        [HttpDelete("{noteId}")]
        [Authorize(Policy = "Permission:TaskNote.Delete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteNote(Guid id, Guid noteId)
        {
            var command = new RemoveTaskNoteCommand(id, noteId);
            await Mediator.Send(command);

            return Success("NoteDeletedSuccessfully");
        }

    }
}
