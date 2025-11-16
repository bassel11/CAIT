using CommitteeApplication.Features.CommitteeMembers.Commands.Models;
using CommitteeApplication.Features.CommitteeMembers.Queries.Models;
using CommitteeApplication.Features.CommitteeMembers.Queries.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CommitteeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommitteeMemberController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CommitteeMemberController> _logger;

        public CommitteeMemberController(IMediator mediator, ILogger<CommitteeMemberController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}", Name = "GetCommitteeMembersById")]
        [ProducesResponseType(typeof(IEnumerable<CommitteeMemberResponse>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:CommitteeMember.View")]
        public async Task<ActionResult<IEnumerable<CommitteeMemberResponse>>> GetCommitteeMembersById(Guid id)
        {
            var query = new GetComMembersListQuery(id);
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }
        //Just for testing 
        [HttpPost(Name = "AddCommitteeMember")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:CommitteeMember.Create")]
        public async Task<ActionResult<int>> AddCommitteeMember([FromBody] AddCommitteeMemberCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPut(Name = "UpdateCommitteeMember")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = "Permission:CommitteeMember.Update")]
        public async Task<ActionResult<int>> UpdateCommitteeMember([FromBody] UpdateCommitteeMemberCommand command)
        {
            var result = await _mediator.Send(command);
            return NoContent();
        }
        [HttpDelete("{id}", Name = "DeleteCommitteeMember")]
        [Authorize(Policy = "Permission:CommitteeMember.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCommitteeMember(Guid id)
        {
            var cmd = new DeleteCommitteeMemberCommand() { Id = id };
            await _mediator.Send(cmd);
            return NoContent();
        }

    }
}
