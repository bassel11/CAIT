using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Wrappers;
using CommitteeApplication.Features.CommitteeQuorumRules.Commands.Models;
using CommitteeApplication.Features.CommitteeQuorumRules.Queries.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommitteeAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/CommitteeQuorumRule")]
    [Authorize]
    public class CommitteeQuorumRuleController : BaseApiController
    {
        #region Fields
        private readonly ILogger<CommitteeQuorumRuleController> _logger;
        #endregion

        #region Constructor
        public CommitteeQuorumRuleController(ILogger<CommitteeQuorumRuleController> logger)
        {
            _logger = logger;
        }
        #endregion

        #region Actions

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        [Authorize(Policy = "Permission:CommitteeQuorumRule.View")]
        //[ProducesResponseType(typeof(Result<IEnumerable<GetQuorumRulesResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new GetQuorumRulesQuery());
            return Success(result);
        }

        // -------------------------------------------------------
        // GET BY ID
        // -------------------------------------------------------
        [HttpGet("{id:guid}", Name = "GetQuorumRuleById")]
        [Authorize(Policy = "Permission:CommitteeQuorumRule.View")]
        //[ProducesResponseType(typeof(Result<GetQuorumRuleByIdResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await Mediator.Send(new GetQuorumRuleByIdQuery(id));
            return Success(result);
        }

        // -------------------------------------------------------
        // GET BY Committee ID
        // -------------------------------------------------------
        [HttpGet("by-committee/{committeeId:guid}")]
        [Authorize(Policy = "Permission:CommitteeQuorumRule.View")]
        //[ProducesResponseType(typeof(Result<GetQuorumRuleByIdResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCommitteeId(Guid committeeId)
        {
            var result = await Mediator.Send(new QuorumRuleByCommitIdQuery(committeeId));
            return Success(result);
        }

        // -------------------------------------------------------
        // CREATE
        // -------------------------------------------------------
        [HttpPost(Name = "CreateCommitteeQuorumRule")]
        [Authorize(Policy = "Permission:CommitteeQuorumRule.Create")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateQuorumRuleCommand command)
        {
            var id = await Mediator.Send(command);
            return CreatedSuccess(
                nameof(GetById),
                new { id = id },
                id,
                "QuorumRuleCreatedSuccessfully");
        }

        // -------------------------------------------------------
        // UPDATE
        // -------------------------------------------------------
        [HttpPut("{id:guid}", Name = "UpdateCommitteeQuorumRule")]
        [Authorize(Policy = "Permission:CommitteeQuorumRule.Update")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuorumRuleCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(Result<string>.Failure(new List<string> { "Id mismatch" }));
            }

            var result = await Mediator.Send(command);
            return EditSuccess(result, "QuorumRuleUpdatedSuccessfully");
        }

        // -------------------------------------------------------
        // DELETE
        // -------------------------------------------------------
        [HttpDelete("{id:guid}", Name = "DeleteCommitteeQuorumRule")]
        [Authorize(Policy = "Permission:CommitteeQuorumRule.Delete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var cmd = new DeleteQuorumRuleCommand() { Id = id };
            await Mediator.Send(cmd);
            return Success("QuorumRuleDeletedSuccessfully");
        }

        #endregion

    }
}
