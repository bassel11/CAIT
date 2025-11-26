using MediatR;
using MeetingApplication.Features.Attendances.Commands.Models;
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
        //[HttpPost("bulk/check-in")]
        //[Authorize(Policy = "Permission:Attendance.CheckIn")]
        //public async Task<ActionResult> BulkCheckIn([FromBody] BulkCheckInDto dto)
        //{
        //    var entries = dto.Entries
        //                     .Select(e => (e.MemberId, e.Status))
        //                     .ToList();

        //    var command = new BulkCheckInCommand(dto.MeetingId, entries);

        //    await _mediator.Send(command);
        //    return NoContent();
        //}


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

        #endregion
    }
}
