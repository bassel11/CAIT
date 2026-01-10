using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Exceptions;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingApplication.Features.MoMs.Queries.Models;
using MeetingApplication.Features.MoMs.Queries.Results;
using MeetingApplication.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/MoM")]
    [Authorize]
    public class MoMController : BaseApiController
    {

        #region Actions

        // -------------------------------------------------------------
        // CREATE
        // -------------------------------------------------------------
        [HttpPost]
        [Authorize(Policy = "Permission:MoM.Create")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateMoMCommand command)
        {
            var id = await Mediator.Send(command);

            return CreatedSuccess(
                nameof(GetById),
                new { id, version = "1.0" },
                id,
                "MoMCreatedSuccessfully");
        }

        // -------------------------------------------------------------
        // UPDATE
        // -------------------------------------------------------------
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "Permission:MoM.Update")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMoMCommand command)
        {
            if (id != command.MoMId)
                throw new BadRequestException("ID mismatch");

            var resultId = await Mediator.Send(command);

            return Success(resultId, "MoMUpdatedSuccessfully");
        }

        // -------------------------------------------------------------
        // SUBMIT (Send for Approval)
        // -------------------------------------------------------------
        [HttpPut("{id:guid}/submit")]
        [Authorize(Policy = "Permission:MoM.Submit")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Submit(Guid id)
        {
            // نفترض أن الـ Command يستقبل ID (يفضل استخدام Record)
            await Mediator.Send(new SubmitMoMForApprovalCommand(id));

            return Success("MoMSubmittedSuccessfully");
        }

        // -------------------------------------------------------------
        // APPROVE
        // -------------------------------------------------------------
        [HttpPut("{id:guid}/approve")]
        [Authorize(Policy = "Permission:MoM.Approve")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Approve(Guid id)
        {
            await Mediator.Send(new ApproveMoMCommand(id));

            return Success("MoMApprovedSuccessfully");
        }

        // -------------------------------------------------------------
        // REJECT
        // -------------------------------------------------------------
        [HttpPut("{id:guid}/reject")]
        [Authorize(Policy = "Permission:MoM.Reject")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectMoMCommand command)
        {
            if (id != command.Id)
                throw new BadRequestException("ID mismatch");

            var resultId = await Mediator.Send(command);

            return Success(resultId, "MoMRejectedSuccessfully");
        }

        // -------------------------------------------------------------
        // GET By ID
        // -------------------------------------------------------------
        [HttpGet("{id:guid}")]
        [Authorize(Policy = "Permission:MoM.View")]
        [ProducesResponseType(typeof(Result<GetMinutesResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await Mediator.Send(new GetMoMByIdQuery { MoMId = id });

            // لا حاجة لفحص null، الـ Handler يرمي NotFoundException
            return Success(result, "MoMRetrievedSuccessfully");
        }

        // -------------------------------------------------------------
        // GET By Meeting ID
        // -------------------------------------------------------------
        [HttpGet("meeting/{meetingId:guid}")]
        [Authorize(Policy = "Permission:MoM.View")]
        [ProducesResponseType(typeof(Result<List<GetMinutesResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByMeetingId(Guid meetingId)
        {
            var result = await Mediator.Send(new GetMoMsForMeetingQuery { MeetingId = meetingId });

            return Success(result, "MoMsRetrievedSuccessfully");
        }

        // -------------------------------------------------------
        // Search (Paginated)
        // -------------------------------------------------------
        [HttpPost("search")]
        [Authorize(Policy = "Permission:MoM.View")]
        [ProducesResponseType(typeof(Result<PaginatedResult<GetMinutesResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMoMs([FromBody] GetMoMsByMeetingQuery query)
        {
            var result = await Mediator.Send(query);
            return Success(result);
        }

        #endregion
    }
}
