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
        // 1. POST: Add Attendee (Invite)
        // -------------------------------------------------------------
        [HttpPost]
        [Authorize(Policy = "Permission:Attendance.Add")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Add([FromBody] AddAttendeeCommand command)
        {
            var result = await Mediator.Send(command);
            return Success(result, "Attendee Added Successfully");
        }

        // -------------------------------------------------------------
        // 2. DELETE: Remove Attendee
        // -------------------------------------------------------------
        // التعديل: نحتاج معرف الاجتماع ومعرف العضو للحذف بدقة
        [HttpDelete("meeting/{meetingId:guid}/user/{userId:guid}")]
        [Authorize(Policy = "Permission:Attendance.Remove")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove(Guid meetingId, Guid userId)
        {
            await Mediator.Send(new RemoveAttendeeCommand(meetingId, userId));
            return Success("Attendee Removed Successfully");
        }

        // -------------------------------------------------------------
        // 3. PUT: Confirm RSVP (Member Action)
        // -------------------------------------------------------------
        // المستخدم يؤكد حضوره بنفسه (UserId يُحقن في الـ Handler)
        [HttpPut("rsvp")]
        [Authorize(Policy = "Permission:Attendance.Confirm")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConfirmRSVP([FromBody] ConfirmAttendanceCommand command)
        {
            var result = await Mediator.Send(command);
            return Success(result, "RSVP Confirmed Successfully");
        }

        // -------------------------------------------------------------
        // 4. PUT: Check-In (Individual)
        // -------------------------------------------------------------
        [HttpPut("check-in")]
        [Authorize(Policy = "Permission:Attendance.CheckIn")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckIn([FromBody] CheckInAttendeeCommand command)
        {
            var result = await Mediator.Send(command);
            return Success(result, "Checked-In Successfully");
        }

        // -------------------------------------------------------------
        // 5. POST: Bulk Check-In
        // -------------------------------------------------------------
        [HttpPost("bulk-check-in")] // Kebab-case naming convention
        [Authorize(Policy = "Permission:Attendance.CheckIn")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> BulkCheckIn([FromBody] BulkCheckInCommand command)
        {
            await Mediator.Send(command);
            return Success("Bulk Check-In Completed Successfully");
        }




        // -------------------------------------------------------------
        // 6. GET: Meeting Attendees (Search via POST)
        // -------------------------------------------------------------
        // قمنا بتغييره لـ POST لنتمكن من استقبال الـ Pagination في الـ Body
        [HttpPost("meeting/{meetingId:guid}/attendees/search")]
        [Authorize(Policy = "Permission:Attendance.View")]
        [ProducesResponseType(typeof(Result<PaginatedResult<AttendanceResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMeetingAttendees(Guid meetingId, [FromBody] GetMeetingAttendeesQuery query)
        {
            query.MeetingId = meetingId;
            var result = await Mediator.Send(query);
            return Success(result);
        }

        // -------------------------------------------------------------
        // 7. GET: Member History (Search via POST)
        // -------------------------------------------------------------
        // قمنا بتغييره لـ POST لنتمكن من استقبال الـ Pagination في الـ Body
        [HttpPost("member/{memberId:guid}/history/search")]
        [Authorize(Policy = "Permission:Attendance.View")]
        [ProducesResponseType(typeof(Result<PaginatedResult<AttendanceResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMemberHistory(Guid memberId, [FromBody] GetMemberAttendanceHistoryQuery query)
        {
            query.MemberId = memberId;
            var result = await Mediator.Send(query);
            return Success(result);
        }






        // -------------------------------------------------------------
        // GET: By Member (Paginated)
        // -------------------------------------------------------------
        //[HttpGet("member")] // نستخدم Query String للبحث
        //[Authorize(Policy = "Permission:Attendance.View")]
        //[ProducesResponseType(typeof(Result<PaginatedResult<AttendanceResponse>>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetAttendanceForMember([FromQuery] GetAttendanceForMemberQuery query)
        //{
        //    var result = await Mediator.Send(query);
        //    return Success(result);
        //}

        // -------------------------------------------------------------
        // GET: Validate Quorum
        // -------------------------------------------------------------
        [HttpGet("meeting/{meetingId:guid}/quorum")]
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
