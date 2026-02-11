using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Exceptions;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingApplication.Features.Meetings.Queries.Models;
using MeetingApplication.Features.Meetings.Queries.Results;
using MeetingApplication.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Meeting")]
    [Authorize]
    public class MeetingController : BaseApiController
    {


        #region #region Writes (Commands)

        // -------------------------------------------------------
        // Craete Meeting
        // -------------------------------------------------------
        [HttpPost]
        [Authorize(Policy = "Permission:Meeting.Create")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateMeetingCommand command)
        {
            var response = await Mediator.Send(command);
            if (response.Succeeded)
            {
                return CreatedAtAction(
                    nameof(GetById), new
                    {
                        id = response.Data,
                        version = "1.0"
                    },
                    response);
            }
            return BadRequest(response);
        }

        // -------------------------------------------------------
        // Update Details
        // -------------------------------------------------------
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "Permission:Meeting.Update")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMeetingCommand command)
        {
            if (id != command.Id)
                return BadRequest(Result<Guid>.Failure("ID mismatch"));
            var response = await Mediator.Send(command);

            if (response.Succeeded)
                return Ok(response);

            return BadRequest(response);
        }

        // -------------------------------------------------------
        // Schedule Meeting
        // -------------------------------------------------------
        [HttpPut("{id:guid}/schedule")]
        [Authorize(Policy = "Permission:Meeting.Schedule")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Schedule(Guid id)
        {
            var response = await Mediator.Send(new ScheduleMeetingCommand(id));
            if (response.Succeeded)
                return Ok(response);

            return BadRequest(response);
        }

        // -------------------------------------------------------
        // Reschedule Meeting
        // -------------------------------------------------------
        [HttpPut("{id:guid}/reschedule")]
        [Authorize(Policy = "Permission:Meeting.Reschedule")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleMeetingCommand command)
        {
            if (id != command.Id)
                throw new BadRequestException("ID mismatch");

            var response = await Mediator.Send(command);

            if (response.Succeeded)
                return Ok(response);

            return BadRequest(response);
        }


        // -------------------------------------------------------
        // Cancel Meeting
        // -------------------------------------------------------
        [HttpPut("{id:guid}/cancel")]
        [Authorize(Policy = "Permission:Meeting.Cancel")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelMeetingRequestDto request)
        {
            var response = await Mediator.Send(new CancelMeetingCommand(id, request.Reason));
            if (response.Succeeded)
                return Ok(response);

            return BadRequest(response);
        }


        // -------------------------------------------------------
        // Complete Meeting
        // -------------------------------------------------------
        [HttpPut("{id:guid}/complete")]
        [Authorize(Policy = "Permission:Meeting.Complete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Complete(Guid id)
        {
            var response = await Mediator.Send(new CompleteMeetingCommand(id));
            if (response.Succeeded)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPut("{id}/refresh-quorum")]
        [Authorize(Policy = "Permission:Meeting.Create")] // صلاحيات حساسة
        public async Task<IActionResult> RefreshQuorum(Guid id)
        {
            var command = new RefreshMeetingQuorumCommand(id);
            var result = await Mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("{id}/check-quorum-updates")]
        [Authorize(Policy = "Permission:Meeting.Create")]
        public async Task<IActionResult> CheckQuorumUpdates(Guid id)
        {
            var query = new CheckQuorumMismatchQuery(id);
            var response = await Mediator.Send(query);
            if (response.Succeeded)
                return Ok(response);

            return BadRequest(response);
        }

        #endregion


        #region Reads (Queries)

        // -------------------------------------------------------
        // Get By ID
        // -------------------------------------------------------
        [HttpGet("{id:guid}")]
        [Authorize(Policy = "Permission:Meeting.View")]
        [ProducesResponseType(typeof(Result<GetMeetingResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await Mediator.Send(new GetMeetingByIdQuery(id));
            if (response.Succeeded)
                return Ok(response);

            return BadRequest(response);
        }


        // -------------------------------------------------------
        // Search (POST for complex filters)
        // -------------------------------------------------------
        [HttpPost("search")] // تغيير الاسم ليكون أوضح
        [Authorize(Policy = "Permission:Meeting.View")]
        [ProducesResponseType(typeof(Result<PaginatedResult<GetMeetingResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMeetings([FromBody] GetMeetingsQuery query)
        {
            var response = await Mediator.Send(query);
            return Ok(response);
        }

        // -------------------------------------------------------
        // Get By Committee (Helper)
        // -------------------------------------------------------
        [HttpGet("committee/{committeeId:guid}")]
        [Authorize(Policy = "Permission:Meeting.View")]
        [ProducesResponseType(typeof(Result<List<GetMeetingResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCommitteeId(Guid committeeId)
        {
            var response = await Mediator.Send(new GetMeetingsByCommitteeIdQuery(committeeId));
            return Ok(response);
        }

        #endregion
    }
}
