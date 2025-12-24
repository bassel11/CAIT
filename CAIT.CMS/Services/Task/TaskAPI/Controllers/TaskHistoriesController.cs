using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApplication.Features.Histories.Queries.GetHistory;

namespace TaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskHistoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TaskHistoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}/history")]
        [Authorize(Policy = "Permission:TaskHsitory.View")]
        public async Task<IActionResult> GetTaskHistory(Guid id)
        {
            var result = await _mediator.Send(new GetTaskHistoryQuery(id));
            return Ok(result);
        }
    }
}
