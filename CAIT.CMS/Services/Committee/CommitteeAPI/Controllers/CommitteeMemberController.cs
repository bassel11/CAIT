using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using CommitteeApplication.Features.CommitteeMembers.Commands.Models;
using CommitteeApplication.Features.CommitteeMembers.Commands.Results;
using CommitteeApplication.Features.CommitteeMembers.Queries.Models;
using CommitteeApplication.Features.CommitteeMembers.Queries.Results;
using CommitteeApplication.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CommitteeAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/CommitteeMember")]

    //[Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class CommitteeMemberController : BaseApiController
    {
        #region Fields
        private readonly IMediator _mediator;
        private readonly ILogger<CommitteeMemberController> _logger;
        #endregion

        #region Constructor
        public CommitteeMemberController(IMediator mediator, ILogger<CommitteeMemberController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        #endregion

        #region Actions

        [HttpGet("{committeeId}", Name = "GetCommitteeMembersById")]
        [ProducesResponseType(typeof(IEnumerable<CommitteeMemberResponse>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:CommitteeMember.View")]
        public async Task<ActionResult<IEnumerable<CommitteeMemberResponse>>> GetCommitteeMembersById(Guid committeeId)
        {
            var query = new GetComMembersListQuery(committeeId);
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = "Permission:CommitteeMember.Update")]
        public async Task<ActionResult<int>> UpdateCommitteeMember([FromBody] UpdateCommitteeMemberCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new
            {
                message = "Committee Member updated successfully"
            });
        }
        [HttpDelete("{id}", Name = "DeleteCommitteeMember")]
        [Authorize(Policy = "Permission:CommitteeMember.Delete")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCommitteeMember(Guid id)
        {
            var cmd = new DeleteCommitteeMemberCommand() { Id = id };
            await _mediator.Send(cmd);
            return Ok(new
            {
                message = "Committee Member Deleted successfully"
            });
        }


        [HttpPost("AssignMultipleCommitteeMembers", Name = "AssignCommitteeMembers")]
        [ProducesResponseType(typeof(AssignCommitteeMembersResult), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:CommitteeMember.Create")]
        public async Task<ActionResult<AssignCommitteeMembersResult>> AssignCommitteeMembers([FromBody] AssignCommitteeMembersCommand command)
        {
            var result = await _mediator.Send(command);

            //if (result.AddedMemberIds == null || !result.AddedMemberIds.Any())
            //    return BadRequest("No new members were added. They might already exist in the committee.");

            return Ok(result);
        }



        [HttpPost("RemoveCommitteeMembers", Name = "RemoveCommitteeMembers")]
        [ProducesResponseType(typeof(RemoveCommitteeMembersResult), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:CommitteeMember.Delete")]
        public async Task<ActionResult<RemoveCommitteeMembersResult>> RemoveCommitteeMembers([FromBody] RemoveCommitteeMembersCommand command)
        {
            //if (command.MembersIds == null || !command.MembersIds.Any())
            //    return BadRequest("No member IDs provided for removal.");

            var result = await _mediator.Send(command);

            //if (!result.RemovedMemberIds.Any() && result.NotFoundMemberIds.Any())
            //    return NotFound(new
            //    {
            //        message = "No members were removed. All provided IDs were not found in the committee.",
            //        notFoundMemberIds = result.NotFoundMemberIds
            //    });

            return Ok(result);
        }

        [HttpPost("filtered", Name = "GetFilteredCommitteeMembers")]
        [ProducesResponseType(typeof(PaginatedResult<CommitMembsFilterResponse>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:Committee.View")]
        public async Task<ActionResult<PaginatedResult<CommitMembsFilterResponse>>> GetFilteredCommittees([FromBody] GetComitMembsFilteredQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpGet("{committeeId:guid}/members/count")]
        [ProducesResponseType(typeof(MemberCountResponse), StatusCodes.Status200OK)]
        [Authorize(Policy = "Permission:CommitteeMember.View")]
        public async Task<IActionResult> GetMemberCount(Guid committeeId, CancellationToken ct)
        {
            var result = await _mediator.Send(new MemberCountQuery(committeeId), ct);
            return Ok(result);
        }


        #endregion
    }
}
