using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Wrappers;
using CommitteeApplication.Features.StatusHistories.Commands.Models;
using CommitteeApplication.Features.StatusHistories.Queries.Models;
using CommitteeApplication.Features.StatusHistories.Queries.Results;
using CommitteeApplication.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommitteeAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/CommitteeStatusHistory")]
    [Authorize]
    public class CommitteeStatusHistoryController : BaseApiController
    {

        #region Constructors
        public CommitteeStatusHistoryController()
        {
        }
        #endregion

        #region Actions
        // -------------------------------------------------------
        // GET By Committee Id
        // -------------------------------------------------------
        [HttpGet("by-committee/{committeeId}", Name = "GetCommitteeStatusHistories")]
        [Authorize(Policy = "Permission:CommitteeStatusHistory.View")]
        [ProducesResponseType(typeof(Result<IEnumerable<CommitStatusHistoryResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCommitteeId(Guid committeeId)
        {
            var query = new GetCommitStatusHistoryQuery { CommitteeId = committeeId };
            var result = await Mediator.Send(query);

            return Success(result);
        }

        // -------------------------------------------------------
        // GET Filtered (Search)
        // -------------------------------------------------------
        [HttpPost("filtered", Name = "GetFilteredStatusHistory")]
        [Authorize(Policy = "Permission:CommitteeStatusHistory.View")]
        [ProducesResponseType(typeof(Result<PaginatedResult<CommitStatusHistoryResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFiltered([FromBody] GetCommitStatusHistoryQuery query)
        {
            var result = await Mediator.Send(query);
            return Success(result);
        }

        // -------------------------------------------------------
        // CREATE
        // -------------------------------------------------------
        [HttpPost(Name = "AddCommitteeStatusHistory")]
        [Authorize(Policy = "Permission:CommitteeStatusHistory.Create")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] AddCommitStatusHistoryCommand command)
        {
            var id = await Mediator.Send(command);
            return Success(id, "StatusHistoryAddedSuccessfully");
        }

        #endregion

    }
}
