using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskApplication.Features.Tasks.Commands.CreateTask;

namespace TaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TaskController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskCommand command)
        {
            var id = await _mediator.Send(command);
            //return CreatedAtAction(nameof(GetById), new { id }, id);
            return Ok();
        }
    }
}
