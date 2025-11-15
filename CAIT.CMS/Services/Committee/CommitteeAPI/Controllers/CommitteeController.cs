using CommitteeApplication.Commands;
using CommitteeApplication.Queries;
using CommitteeApplication.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CommitteeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommitteeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CommitteeController> _logger;

        public CommitteeController(IMediator mediator, ILogger<CommitteeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}", Name = "GetCommitteesById")]
        [ProducesResponseType(typeof(IEnumerable<CommitteeResponse>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:Meeting.Create")]
        public async Task<ActionResult<IEnumerable<CommitteeResponse>>> GetCommitteesById(Guid id)
        {
            var query = new GetCommitteeListQuery(id);
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }
        //Just for testing 
        [HttpPost(Name = "AddCommittee")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> AddCommittee([FromBody] AddCommitteeCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPut(Name = "UpdateCommittee")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> UpdateOrder([FromBody] UpdateCommitteeCommand command)
        {
            var result = await _mediator.Send(command);
            return NoContent();
        }
        [HttpDelete("{id}", Name = "DeleteCommittee")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCommittee(Guid id)
        {
            var cmd = new DeleteCommitteeCommand() { Id = id };
            await _mediator.Send(cmd);
            return NoContent();
        }
    }
}
