using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Pagination;
using DecisionApplication.Decisions.Commands.CreateDecision;
using DecisionApplication.Decisions.Commands.DeleteDecision;
using DecisionApplication.Decisions.Commands.UpdateDecision;
using DecisionApplication.Decisions.Queries.GetDecisionById;
using DecisionApplication.Decisions.Queries.GetDecisions;
using DecisionApplication.Decisions.Queries.GetDecisionsByMeeting;
using DecisionApplication.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DecisionAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Decisions")]

    //[Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class DecisionsController : BaseApiController
    {
        private readonly ISender _mediator;

        public DecisionsController(ISender mediator)
        {
            _mediator = mediator;
        }

        // Create Decision
        [HttpPost]
        [Authorize(Policy = "Permission:Decision.Create")]
        public async Task<IActionResult> Create([FromBody] CreateDecisionDto createdecisionDto)
        {
            var command = new CreateDecisionCommand(createdecisionDto);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // Update Decision
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "Permission:Decision.Update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDecisionDto updatedecisionDto)
        {
            var command = new UpdateDecisionCommand(id, updatedecisionDto);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Delete Decision
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "Permission:Decision.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteDecisionCommand(id);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Get paginated decisions
        [HttpGet]
        [Authorize(Policy = "Permission:Decision.View")]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            var query = new GetDecisionsQuery(new PaginationRequest(pageIndex, pageSize));
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Get decisions by meeting
        [HttpGet("meeting/{meetingId:guid}")]
        [Authorize(Policy = "Permission:Decision.View")]
        public async Task<IActionResult> GetByMeeting(Guid meetingId)
        {
            var query = new GetDecisionsByMeetingQuery(meetingId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Get single decision by Id
        [HttpGet("{id:guid}")]
        [Authorize(Policy = "Permission:Decision.View")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var decision = await _mediator.Send(new GetDecisionByIdQuery(id));
            if (decision == null) return NotFound();
            return Ok(decision);
        }

        // Optional: Add Vote to Decision
        [HttpPost("{id:guid}/votes")]
        [Authorize(Policy = "Permission:Vote.Create")]
        public async Task<IActionResult> AddVote(Guid id, [FromBody] VoteDto voteDto)
        {
            // هنا يمكن إرسال أمر AddVoteCommand
            // مثال: var command = new AddVoteCommand(id, voteDto.MemberId, voteDto.Type);
            // var result = await _mediator.Send(command);
            // return Ok(result);

            return StatusCode(501, "Voting endpoint not implemented yet.");
        }
    }
}
