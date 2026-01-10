using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Pagination;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Attendances.Commands.Models;
using MeetingApplication.Features.Attendances.Queries.Models;
using MeetingApplication.Features.Attendances.Queries.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Attendance")]

    //[Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class AttendanceController : BaseApiController
    {

        #region Actions

        // -------------------------------------------------------------
        // POST: Check-In
        // -------------------------------------------------------------
        [HttpPost("check-in")]
        [Authorize(Policy = "Permission:Attendance.CheckIn")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckIn([FromBody] CheckInAttendanceCommand command)
        {
            var id = await Mediator.Send(command);
            return Success(id, "AttendanceCheckedInSuccessfully");
        }

        // -------------------------------------------------------------
        // POST: Confirm
        // -------------------------------------------------------------
        [HttpPost("confirm")]
        [Authorize(Policy = "Permission:Attendance.Confirm")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Confirm([FromBody] ConfirmAttendanceCommand command)
        {
            var id = await Mediator.Send(command);
            return Success(id, "AttendanceConfirmedSuccessfully");
        }

        // -------------------------------------------------------------
        // POST: Bulk Check-In
        // -------------------------------------------------------------
        [HttpPost("bulk/check-in")]
        [Authorize(Policy = "Permission:Attendance.CheckIn")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> BulkCheckIn([FromBody] BulkCheckInCommand command)
        {
            // ❌ حذفنا التحقق اليدوي (if null)
            // ✅ FluentValidation سيقوم بالتحقق من أن القائمة ليست فارغة

            await Mediator.Send(command);
            return Success("BulkAttendanceCheckedInSuccessfully");
        }

        // -------------------------------------------------------------
        // DELETE: Remove
        // -------------------------------------------------------------
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "Permission:Attendance.Remove")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove(Guid id)
        {
            await Mediator.Send(new RemoveAttendanceCommand(id));
            return Success("AttendanceRemovedSuccessfully");
        }

        // -------------------------------------------------------------
        // GET: By Meeting ID
        // -------------------------------------------------------------
        [HttpGet("meeting/{meetingId:guid}")]
        [Authorize(Policy = "Permission:Attendance.View")]
        [ProducesResponseType(typeof(Result<List<GetAttendanceResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAttendanceForMeeting(Guid meetingId)
        {
            var result = await Mediator.Send(new GetAttendanceForMeetingQuery(meetingId));
            return Success(result);
        }

        // -------------------------------------------------------------
        // GET: By Member (Paginated)
        // -------------------------------------------------------------
        [HttpGet("member")] // نستخدم Query String للبحث
        [Authorize(Policy = "Permission:Attendance.View")]
        [ProducesResponseType(typeof(Result<PaginatedResult<GetAttendanceResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAttendanceForMember([FromQuery] GetAttendanceForMemberQuery query)
        {
            var result = await Mediator.Send(query);
            return Success(result);
        }

        // -------------------------------------------------------------
        // GET: Validate Quorum
        // -------------------------------------------------------------
        [HttpGet("quorum/{meetingId:guid}")] // ✅ اختصار المسار
        [Authorize(Policy = "Permission:Attendance.ValidateQuorum")]
        [ProducesResponseType(typeof(Result<QuorumValidationResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ValidateQuorum(Guid meetingId)
        {
            var result = await Mediator.Send(new ValidateQuorumQuery(meetingId));
            return Success(result);
        }


        #endregion
    }
}
