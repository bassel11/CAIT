using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApplication.Dtos;
using TaskApplication.Features.Tasks.Queries.GetTasks;

namespace TaskAPI.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/tasks")]

    [Authorize]
    public class TasksController : BaseApiController
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
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
