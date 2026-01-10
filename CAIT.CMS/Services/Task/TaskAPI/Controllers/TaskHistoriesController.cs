using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApplication.Dtos;
using TaskApplication.Features.Histories.Queries.GetHistory;

namespace TaskAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/TaskHistories")]
    [Authorize]
    public class TaskHistoriesController : BaseApiController
    {

        [HttpGet("{taskId}")]
        [Authorize(Policy = "Permission:TaskHistory.View")]
        [ProducesResponseType(typeof(Result<List<TaskHistoryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTaskHistory(Guid taskId)
        {
            var result = await Mediator.Send(new GetTaskHistoryQuery(taskId));

            return Success(result, "HistoryRetrievedSuccessfully");
        }
    }
}