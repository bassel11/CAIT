using MediatR;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingApplication.Features.Meetings.Commands.Results;
using MeetingApplication.Features.Meetings.Queries.Models;
using MeetingApplication.Features.Meetings.Queries.Results;
using MeetingApplication.Responses;
using MeetingApplication.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MeetingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MeetingController : ControllerBase
    {
        #region Fields
        private readonly IMediator _mediator;
        private readonly ILogger<MeetingController> _logger;
        #endregion

        #region Constructor
        public MeetingController(IMediator mediator
                                   , ILogger<MeetingController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        #endregion

        #region Actions

        // -------------------------------------------------------
        // CREATE Meeting
        // -------------------------------------------------------
        [HttpPost(Name = "CreateMeeting")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:Meeting.Create")]
        public async Task<ActionResult<CreateMeetingResponse>> Create([FromBody] CreateMeetingCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // -------------------------------------------------------
        // UPDATE Meeting
        // -------------------------------------------------------
        [HttpPut("{id:guid}", Name = "UpdateMeeting")]
        [Authorize(Policy = "Permission:Meeting.Update")]
        public async Task<ActionResult<UpdateMeetingResponse>> Update(Guid id, [FromBody] UpdateMeetingCommand command)
        {
            if (id != command.Id)
                return BadRequest("Id mismatch");

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // -------------------------------------------------------
        // CANCEL Meeting
        // -------------------------------------------------------
        [HttpDelete("{id:guid}/cancel", Name = "CancelMeeting")]
        [Authorize(Policy = "Permission:Meeting.Cancel")]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelMeetingCommand command)
        {

            if (id != command.Id)
                return BadRequest("ID mismatch");

            await _mediator.Send(command);
            return Ok(new
            {
                message = "Meeting Canceled successfully"
            });
        }


        // -------------------------------------------------------
        // Complete Meeting
        // -------------------------------------------------------
        [HttpPut("{id:guid}/complete", Name = "CompleteMeeting")]
        [Authorize(Policy = "Permission:Meeting.Complete")]
        public async Task<IActionResult> Complete(Guid id, [FromBody] CompleteMeetingCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            await _mediator.Send(command);
            return Ok(new
            {
                message = "Meeting Completed successfully"
            });
        }


        // -------------------------------------------------------
        // Reschedule Meeting
        // -------------------------------------------------------
        [HttpPut("{id:guid}/reschedule", Name = "RescheduleMeeting")]
        [Authorize(Policy = "Permission:Meeting.Reschedule")]
        public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleMeetingCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            await _mediator.Send(command);
            return Ok(new
            {
                message = "Meeting Completed successfully"
            });
        }


        // -------------------------------------------------------
        // Get Meeting By Id
        // -------------------------------------------------------
        [HttpGet("GetMeetingById/{id:guid}")]
        [Authorize(Policy = "Permission:Meeting.View")]
        [ProducesResponseType(typeof(Response<GetMeetingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Response<GetMeetingResponse>>> GetMeetingById(Guid id)
        {
            var query = new GetMeetingByIdQuery(id);
            var response = await _mediator.Send(query);

            if (!response.Succeeded)
                return NotFound(response);

            return Ok(response);
        }


        // -------------------------------------------------------
        // Get Paginated and filtered and Sorted Meeting
        // -------------------------------------------------------
        [HttpPost("GetMeeting", Name = "GetMeeting")]
        [ProducesResponseType(typeof(PaginatedResult<GetMeetingResponse>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:Meeting.View")]
        public async Task<ActionResult<PaginatedResult<GetMeetingResponse>>> GetMeetings([FromBody] GetMeetingsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // -------------------------------------------------------
        // Get Meeting By CommitteeId
        // -------------------------------------------------------
        [HttpGet("GetMeetingByCommitteeId/{id:guid}")]
        [Authorize(Policy = "Permission:Meeting.View")]
        [ProducesResponseType(typeof(Response<GetMeetingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Response<GetMeetingResponse>>> GetMeetingByCommitteeId(Guid committeeId, CancellationToken cancellationToken)
        {
            var query = new GetMeetingsByCommitteeIdQuery(committeeId);
            var response = await _mediator.Send(query);

            if (!response.Succeeded)
                return NotFound(response);

            return Ok(response);
        }

        #endregion
    }
}
