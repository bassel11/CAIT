using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Models;
using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Results;
using CommitteeApplication.Features.CommitteeMemberRoles.Queries.Models;
using CommitteeApplication.Features.CommitteeMemberRoles.Queries.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CommitteeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommitteeMemberRoleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CommitteeMemberRoleController> _logger;

        public CommitteeMemberRoleController(IMediator mediator, ILogger<CommitteeMemberRoleController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{committeeMemberId}", Name = "GetRolesByMemberId")]
        [ProducesResponseType(typeof(IEnumerable<GetCommiMembRolesResponse>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:CommitteeMemberRole.View")]
        public async Task<ActionResult<IEnumerable<GetCommiMembRolesResponse>>> GetRolesByMemberId(Guid committeeMemberId)
        {
            var query = new GetCommiMembRolesQuery(committeeMemberId);
            var data = await _mediator.Send(query);
            return Ok(data);
        }

        [HttpPost("AddMemberRoles")]
        [Authorize(Policy = "Permission:CommitteeMemberRole.Create")]
        public async Task<ActionResult<AddCommiMembRolesResult>> AddMemberRoles(
             [FromBody] AddCommiMembRolesCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPut("UpdateMemberRole", Name = "UpdateCommitteeMemberRole")]
        [Authorize(Policy = "Permission:CommitteeMemberRole.Update")]
        public async Task<ActionResult<UpdateCommiMembRolesResult>> UpdateMemberRole([FromBody] UpdateCommiMembRolesCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("DeleteMemberRole/{id}", Name = "DeleteCommitteeMemberRole")]
        [Authorize(Policy = "Permission:CommitteeMemberRole.Delete")]
        public async Task<ActionResult<DeleteCommiMembRolesResult>> DeleteMemberRole(Guid id)
        {
            var command = new DeleteCommiMembRolesCommand { Id = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

    }
}
