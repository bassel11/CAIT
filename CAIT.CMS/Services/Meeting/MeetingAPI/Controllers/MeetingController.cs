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

    //[Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class MeetingController : BaseApiController
    {


        #region Actions

        // -------------------------------------------------------
        // CREATE Meeting
        // -------------------------------------------------------
        [HttpPost]
        [Authorize(Policy = "Permission:Meeting.Create")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateMeetingCommand command)
        {
            var response = await Mediator.Send(command);

            // ✅ استخدام CreatedSuccess الموحدة
            return CreatedSuccess(
                nameof(GetById),
                new { id = response.Id, version = "1.0" }, // ✅ التصحيح هنا: استخراج الـ ID
                response.Id,
                "MeetingCreatedSuccessfully");
        }

        // -------------------------------------------------------
        // UPDATE Meeting
        // -------------------------------------------------------
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "Permission:Meeting.Update")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMeetingCommand command)
        {
            if (id != command.Id)
                throw new BadRequestException("ID mismatch"); // ✅ استخدام استثناء موحد

            var result = await Mediator.Send(command);

            return Success(result, "MeetingUpdatedSuccessfully");
        }

        // -------------------------------------------------------
        // CANCEL Meeting
        // -------------------------------------------------------
        [HttpDelete("{id:guid}/cancel")]
        [Authorize(Policy = "Permission:Meeting.Cancel")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Cancel(Guid id)
        {
            // نمرر الـ ID للـ Command (يفضل أن يكون الـ Command يستقبل ID في الـ Constructor)
            await Mediator.Send(new CancelMeetingCommand(id));

            return Success("MeetingCanceledSuccessfully");
        }


        // -------------------------------------------------------
        // COMPLETE Meeting
        // -------------------------------------------------------
        [HttpPut("{id:guid}/complete")]
        [Authorize(Policy = "Permission:Meeting.Complete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Complete(Guid id)
        {
            await Mediator.Send(new CompleteMeetingCommand(id));
            return Success("MeetingCompletedSuccessfully");
        }


        // -------------------------------------------------------
        // RESCHEDULE Meeting
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
        // GET Meeting By Id
        // -------------------------------------------------------
        [HttpGet("{id:guid}")] // ✅ مسار نظيف (RESTful)
        [Authorize(Policy = "Permission:Meeting.View")]
        [ProducesResponseType(typeof(Result<GetMeetingResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await Mediator.Send(new GetMeetingByIdQuery(id));

            // لا حاجة لفحص الـ NULL هنا، الـ Handler سيرمي NotFoundException إذا لم يجدها
            return Success(result, "MeetingRetrievedSuccessfully");
        }


        // -------------------------------------------------------
        // GET Paginated Meetings
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
        // GET By CommitteeId
        // -------------------------------------------------------
        [HttpGet("committee/{committeeId:guid}")] // ✅ مسار نظيف
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
