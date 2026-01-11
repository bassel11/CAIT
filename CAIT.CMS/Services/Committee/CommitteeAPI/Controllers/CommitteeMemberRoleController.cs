using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Wrappers;
using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Models;
using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Results;
using CommitteeApplication.Features.CommitteeMemberRoles.Queries.Models;
using CommitteeApplication.Features.CommitteeMemberRoles.Queries.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommitteeAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/CommitteeMemberRole")]
    [Authorize]
    public class CommitteeMemberRoleController : BaseApiController
    {
        #region Fields
        private readonly ILogger<CommitteeMemberRoleController> _logger;
        #endregion

        #region Constructors
        public CommitteeMemberRoleController(ILogger<CommitteeMemberRoleController> logger)
        {
            _logger = logger;
        }
        #endregion

        #region Actions

        // -------------------------------------------------------
        // GET Roles By Member Id
        // -------------------------------------------------------
        [HttpGet("{committeeMemberId}", Name = "GetRolesByMemberId")]
        [Authorize(Policy = "Permission:CommitteeMemberRole.View")]
        [ProducesResponseType(typeof(Result<IEnumerable<GetCommiMembRolesResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRolesByMemberId(Guid committeeMemberId)
        {
            var query = new GetCommiMembRolesQuery(committeeMemberId);
            var data = await Mediator.Send(query);
            return Success(data);
        }

        // -------------------------------------------------------
        // CREATE (Add Roles)
        // -------------------------------------------------------
        [HttpPost(Name = "AddMemberRoles")]
        [Authorize(Policy = "Permission:CommitteeMemberRole.Create")]
        [ProducesResponseType(typeof(Result<AddCommiMembRolesResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddMemberRoles([FromBody] AddCommiMembRolesCommand command)
        {
            var result = await Mediator.Send(command);
            return Success(result, "MemberRolesAddedSuccessfully");
        }

        // -------------------------------------------------------
        // UPDATE
        // -------------------------------------------------------
        [HttpPut(Name = "UpdateCommitteeMemberRole")]
        [Authorize(Policy = "Permission:CommitteeMemberRole.Update")]
        [ProducesResponseType(typeof(Result<UpdateCommiMembRolesResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMemberRole([FromBody] UpdateCommiMembRolesCommand command)
        {
            var result = await Mediator.Send(command);
            return EditSuccess(result, "MemberRoleUpdatedSuccessfully");
        }

        // -------------------------------------------------------
        // DELETE
        // -------------------------------------------------------
        [HttpDelete("{id}", Name = "DeleteCommitteeMemberRole")]
        [Authorize(Policy = "Permission:CommitteeMemberRole.Delete")]
        [ProducesResponseType(typeof(Result<DeleteCommiMembRolesResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMemberRole(Guid id)
        {
            var command = new DeleteCommiMembRolesCommand { Id = id };
            var result = await Mediator.Send(command);
            return Success(result, "MemberRoleDeletedSuccessfully");
        }
        #endregion
    }
}
