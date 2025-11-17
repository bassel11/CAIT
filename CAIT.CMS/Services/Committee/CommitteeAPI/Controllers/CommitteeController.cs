using CommitteeApplication.Features.Committees.Commands.Models;
using CommitteeApplication.Features.Committees.Queries.Models;
using CommitteeApplication.Features.Committees.Queries.Results;
using CommitteeApplication.Wrappers;
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
        [ProducesResponseType(typeof(IEnumerable<GetCommitteeByIdResponse>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:Committee.View")]
        public async Task<ActionResult<IEnumerable<GetCommitteeByIdResponse>>> GetCommitteesById(Guid id)
        {
            var query = new GetCommitteeByIdQuery(id);
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }
        //Just for testing 
        [HttpPost(Name = "AddCommittee")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:Committee.Create")]
        public async Task<ActionResult<int>> AddCommittee([FromBody] AddCommitteeCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPut(Name = "UpdateCommittee")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = "Permission:Committee.Update")]
        public async Task<ActionResult<int>> UpdateOrder([FromBody] UpdateCommitteeCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new
            {
                message = "Committee updated successfully"
            });
        }
        [HttpDelete("{id}", Name = "DeleteCommittee")]
        [Authorize(Policy = "Permission:Committee.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCommittee(Guid id)
        {
            var cmd = new DeleteCommitteeCommand() { Id = id };
            await _mediator.Send(cmd);
            return Ok(new
            {
                message = "Committee deleted successfully"
            });
        }


        [HttpPost("filtered", Name = "GetFilteredCommittees")]
        [ProducesResponseType(typeof(PaginatedResult<GetComitsFilteredResponse>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:Committee.View")]
        public async Task<ActionResult<PaginatedResult<GetComitsFilteredResponse>>> GetFilteredCommittees([FromBody] GetComitsFilteredQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }


    }
}
