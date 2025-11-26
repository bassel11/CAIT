using MediatR;
using MeetingApplication.Features.Attendances.Commands.Models;
using MeetingApplication.Features.Attendances.Queries.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        #region Fields
        private readonly IMediator _mediator;
        private readonly ILogger<AttendanceController> _logger;
        #endregion

        #region Constructor
        public AttendanceController(IMediator mediator
                                   , ILogger<AttendanceController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        #endregion

        #region Actions

        // -------------------------------------------------------------
        // POST: api/attendance/check-in
        // -------------------------------------------------------------
        [HttpPost("check-in")]
        [Authorize(Policy = "Permission:Attendance.CheckIn")]
        public async Task<ActionResult<Guid>> CheckIn([FromBody] CheckInAttendanceCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }


        // -------------------------------------------------------------
        // POST: api/attendance/confirm
        // -------------------------------------------------------------
        [HttpPost("confirm")]
        [Authorize(Policy = "Permission:Attendance.Confirm")]
        public async Task<ActionResult<Guid>> Confirm([FromBody] ConfirmAttendanceCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        // -------------------------------------------------------------
        // POST: api/attendance/bulk/check-in
        // -------------------------------------------------------------
        [HttpPost("bulk/check-in")]
        [Authorize(Policy = "Permission:Attendance.CheckIn")]
        public async Task<IActionResult> BulkCheckIn([FromBody] BulkCheckInCommand dto)
        {
            if (dto.Entries == null || !dto.Entries.Any())
                return BadRequest("Entries cannot be empty.");

            var command = new BulkCheckInCommand
            {
                MeetingId = dto.MeetingId,
                Entries = dto.Entries.Select(x => new BulkCheckInCommand.BulkCheckInEntry
                {
                    MemberId = x.MemberId,
                    Status = x.Status
                }).ToList()
            };

            await _mediator.Send(command);
            return Ok();
        }


        // -------------------------------------------------------------
        // DELETE: api/attendance/{id}
        // -------------------------------------------------------------
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "Permission:Attendance.Remove")]
        public async Task<ActionResult> Remove(Guid id)
        {
            await _mediator.Send(new RemoveAttendanceCommand { Id = id });
            return NoContent();
        }


        // -------------------------------------------------------------
        // GET: api/attendance/meeting/{meetingId}
        // -------------------------------------------------------------
        [HttpGet("meeting/{meetingId}")]
        [Authorize(Policy = "Permission:Attendance.View")]
        public async Task<IActionResult> GetAttendanceForMeeting([FromRoute] Guid meetingId)
        {
            var query = new GetAttendanceForMeetingQuery
            {
                MeetingId = meetingId
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // -------------------------------------------------------------
        // GET: api/attendance/member
        // -------------------------------------------------------------
        [HttpGet("member")]
        [Authorize(Policy = "Permission:Attendance.View")]
        public async Task<IActionResult> GetAttendanceForMember([FromQuery] GetAttendanceForMemberQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result); // PaginatedResult<GetAttendanceResponse>
        }

        // -------------------------------------------------------------
        // GET: api/attendance/validate-quorum/{meetingId}
        // -------------------------------------------------------------
        [HttpGet("validate-quorum/{meetingId}")]
        [Authorize(Policy = "Permission:Attendance.ValidateQuorum")]
        public async Task<IActionResult> ValidateQuorum([FromRoute] Guid meetingId)
        {
            var query = new ValidateQuorumQuery(meetingId);
            var result = await _mediator.Send(query);
            return Ok(result); // QuorumValidationResult
        }


        #endregion
    }
}
