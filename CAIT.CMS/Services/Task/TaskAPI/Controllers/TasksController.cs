using BuildingBlocks.Shared.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApplication.Dtos;
using TaskApplication.Features.Tasks.Commands.AssignUser;
using TaskApplication.Features.Tasks.Commands.CreateTask;
using TaskApplication.Features.Tasks.Commands.UnassignUser;
using TaskApplication.Features.Tasks.Commands.UpdateTaskStatus;
using TaskApplication.Features.Tasks.Commands.UploadAttachment;
using TaskApplication.Features.Tasks.Queries.GetTaskDetails;
using TaskApplication.Features.Tasks.Queries.GetTasks;

namespace TaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Permission:Task.View")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetTaskDetailsQuery(id));
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "Permission:Task.Create")]
        public async Task<IActionResult> Create(CreateTaskCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPost("assignUser")]
        [Authorize(Policy = "Permission:TaskAssignee.Assign")]
        public async Task<IActionResult> AssignUser([FromBody] AssignUserCommand request)
        {
            // نفصل الـ Request Body عن الـ Command لأن الـ ID يأتي من الـ URL
            var command = new AssignUserCommand(request.TaskId, request.UserId, request.Email, request.Name);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("{id}/attachments")]
        [Authorize(Policy = "Permission:TaskAttachment.Upload")]
        public async Task<IActionResult> UploadAttachment(Guid id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using var stream = file.OpenReadStream();

            var command = new UploadAttachmentCommand(
                id,
                stream,
                file.FileName,
                file.ContentType,
                file.Length
            );

            var attachmentId = await _mediator.Send(command);
            return Ok(new { AttachmentId = attachmentId });
        }


        [HttpPut("{id}/UpdateStatus")]
        [Authorize(Policy = "Permission:TaskStatus.Update")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusRequest request)
        {
            // نستخدم الـ ID من المسار لضمان الأمان
            var command = new UpdateTaskStatusCommand(id, request.NewStatus);

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}/assignees/{userId}")]
        [Authorize(Policy = "Permission:TaskAssignee.Unassign")]
        public async Task<IActionResult> UnassignUser(Guid id, Guid userId)
        {
            await _mediator.Send(new UnassignUserCommand(id, userId));
            return NoContent();
        }

        [HttpGet]
        [Authorize(Policy = "Permission:Task.View")]
        [ProducesResponseType(typeof(PaginatedResult<TaskListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTasks(
            [FromQuery] GetTasksQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

    }
}
