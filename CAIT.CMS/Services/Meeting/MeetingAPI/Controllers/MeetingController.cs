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
            return CreatedSuccess(
                nameof(GetById),
                new { id = response.Data, version = "1.0" },
                response.Data,
                "Meeting Created Successfully");
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
                throw new BadRequestException("ID mismatch");
            var result = await Mediator.Send(command);
            return EditSuccess(result, "Meeting Updated Successfully");
        }

        // -------------------------------------------------------
        // Schedule Meeting
        // -------------------------------------------------------
        [HttpPut("{id:guid}/schedule")]
        [Authorize(Policy = "Permission:Meeting.Schedule")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Schedule(Guid id)
        {
            await Mediator.Send(new ScheduleMeetingCommand(id));
            return Success("Meeting Scheduled Successfully");
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

            await Mediator.Send(command);
            return Success("MeetingRescheduledSuccessfully");
        }


        // -------------------------------------------------------
        // Cancel Meeting
        // -------------------------------------------------------
        [HttpPut("{id:guid}/cancel")]
        [Authorize(Policy = "Permission:Meeting.Cancel")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelMeetingRequestDto request)
        {
            await Mediator.Send(new CancelMeetingCommand(id, request.Reason));
            return Success("Meeting Canceled Successfully");
        }


        // -------------------------------------------------------
        // Complete Meeting
        // -------------------------------------------------------
        [HttpPut("{id:guid}/complete")]
        [Authorize(Policy = "Permission:Meeting.Complete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Complete(Guid id)
        {
            await Mediator.Send(new CompleteMeetingCommand(id));
            return Success("Meeting Completed Successfully");
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
            var result = await Mediator.Send(query);
            return Ok(result);
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
            var result = await Mediator.Send(new GetMeetingByIdQuery(id));
            return Success(result, "MeetingRetrievedSuccessfully");
        }


        // -------------------------------------------------------
        // Search (POST for complex filters)
        // -------------------------------------------------------
        [HttpPost("search")] // ✅ تغيير الاسم ليكون أوضح
        [Authorize(Policy = "Permission:Meeting.View")]
        [ProducesResponseType(typeof(Result<PaginatedResult<GetMeetingResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMeetings([FromBody] GetMeetingsQuery query)
        {
            var result = await Mediator.Send(query);
            return Success(result);
        }

        // -------------------------------------------------------
        // Get By Committee (Helper)
        // -------------------------------------------------------
        [HttpGet("committee/{committeeId:guid}")]
        [Authorize(Policy = "Permission:Meeting.View")]
        [ProducesResponseType(typeof(Result<List<GetMeetingResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCommitteeId(Guid committeeId)
        {
            var result = await Mediator.Send(new GetMeetingsByCommitteeIdQuery(committeeId));
            return Success(result);
        }

        #endregion
    }
}
