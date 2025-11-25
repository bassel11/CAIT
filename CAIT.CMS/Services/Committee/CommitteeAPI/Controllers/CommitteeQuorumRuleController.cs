using CommitteeApplication.Features.CommitteeQuorumRules.Commands.Models;
using CommitteeApplication.Features.CommitteeQuorumRules.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CommitteeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommitteeQuorumRuleController : ControllerBase
    {
        #region Fields
        private readonly IMediator _mediator;
        private readonly ILogger<CommitteeQuorumRuleController> _logger;
        #endregion

        #region Constructor
        public CommitteeQuorumRuleController(IMediator mediator, ILogger<CommitteeQuorumRuleController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        #endregion

        #region Actions

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet("All")]
        [Authorize(Policy = "Permission:CommitteeQuorumRule.View")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetQuorumRulesQuery());
            return Ok(result);
        }

        // -------------------------------------------------------
        // GET BY ID
        // -------------------------------------------------------
        [HttpGet("{id:guid}")]
        [Authorize(Policy = "Permission:CommitteeQuorumRule.View")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetQuorumRuleByIdQuery(id));
            if (result == null)
                return NotFound();

            return Ok(result);
        }


        // -------------------------------------------------------
        // GET BY Committee ID
        // -------------------------------------------------------
        [HttpGet("GetByCommitteeId/{committeeId:guid}")]
        [Authorize(Policy = "Permission:CommitteeQuorumRule.View")]
        public async Task<IActionResult> GetByCommitteeId(Guid committeeId)
        {
            var result = await _mediator.Send(new QuorumRuleByCommitIdQuery(committeeId));
            if (result == null)
                return NotFound();

            return Ok(result);
        }


        // -------------------------------------------------------
        // CREATE
        // -------------------------------------------------------
        [HttpPost(Name = "CreateCommitteeQuorumRule")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:CommitteeQuorumRule.Create")]
        public async Task<IActionResult> Create([FromBody] CreateQuorumRuleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        // -------------------------------------------------------
        // UPDATE
        // -------------------------------------------------------
        [HttpPut("{id:guid}", Name = "UpdateCommitteeQuorumRule")]
        [Authorize(Policy = "Permission:CommitteeQuorumRule.Update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuorumRuleCommand command)
        {
            if (id != command.Id)
                return BadRequest("Id mismatch");

            var success = await _mediator.Send(command);

            return Ok(new
            {
                message = "Committee Quorum Rule updated successfully"
            });
        }


        // -------------------------------------------------------
        // DELETE
        // -------------------------------------------------------
        [HttpDelete("{id:guid}", Name = "DeleteCommitteeQuorumRule")]
        [Authorize(Policy = "Permission:CommitteeQuorumRule.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var cmd = new DeleteQuorumRuleCommand() { Id = id };
            await _mediator.Send(cmd);
            return Ok(new
            {
                message = "Committee Quorum Rule Deleted successfully"
            });
        }

        #endregion

    }
}
