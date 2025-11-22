using CommitteeApplication.Features.StatusHistories.Commands.Models;
using CommitteeApplication.Features.StatusHistories.Queries.Models;
using CommitteeApplication.Features.StatusHistories.Queries.Results;
using CommitteeApplication.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CommitteeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommitteeStatusHistoryController : ControllerBase
    {
        #region Fields
        private readonly IMediator _mediator;
        #endregion

        #region Constructors
        public CommitteeStatusHistoryController(IMediator mediator)
        {
            _mediator = mediator;
        }
        #endregion

        #region Actions

        [HttpGet("{committeeId}", Name = "GetCommitteeStatusHistories")]
        [ProducesResponseType(typeof(IEnumerable<CommitStatusHistoryResponse>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:CommitteeStatusHistory.View")]
        public async Task<ActionResult<IEnumerable<CommitStatusHistoryResponse>>> GetByCommitteeId(Guid committeeId)
        {
            var query = new GetCommitStatusHistoryQuery { CommitteeId = committeeId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpPost]
        [ProducesResponseType(typeof(PaginatedResult<CommitStatusHistoryResponse>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:CommitteeStatusHistory.View")]
        public async Task<IActionResult> Get(
            [FromBody] GetCommitStatusHistoryQuery query)
        {
            // "sortBy": "oldStatus.Name:Desc,newStatus.Name:Desc",
            // "oldStatus.Name:eq" : "Draft"
            var result = await _mediator.Send(query);
            return Ok(result);
        }



        [HttpPost("Add", Name = "AddCommitteeStatusHistory")]
        [Authorize(Policy = "Permission:CommitteeStatusHistory.Create")]
        public async Task<ActionResult<Guid>> Add([FromBody] AddCommitStatusHistoryCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }


        #endregion

    }
}
