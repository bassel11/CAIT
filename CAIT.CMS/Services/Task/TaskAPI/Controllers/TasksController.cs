using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskApplication.Features.Tasks.Commands.AssignUser;
using TaskApplication.Features.Tasks.Commands.CreateTask;
using TaskApplication.Features.Tasks.Commands.UpdateTaskStatus;
using TaskApplication.Features.Tasks.Commands.UploadAttachment;
using TaskApplication.Features.Tasks.Queries.GetTaskDetails;

namespace TaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetTaskDetailsQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPost("assignUser")]
        public async Task<IActionResult> AssignUser([FromBody] AssignUserCommand request)
        {
            // نفصل الـ Request Body عن الـ Command لأن الـ ID يأتي من الـ URL
            var command = new AssignUserCommand(request.TaskId, request.UserId, request.Email, request.Name);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("{id}/attachments")]
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
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusRequest request)
        {
            // نستخدم الـ ID من المسار لضمان الأمان
            var command = new UpdateTaskStatusCommand(id, request.NewStatus);

            await _mediator.Send(command);

            return NoContent();
        }


    }
}
