using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMActionItemDrafts.Commands.Models;
using MeetingApplication.Features.MoMActionItemDrafts.Queries.Models;
using MeetingApplication.Features.MoMActionItemDrafts.Queries.Results;
using MeetingApplication.Features.MoMAttachments.Commands.Models;
using MeetingApplication.Features.MoMAttachments.Queries.Models;
using MeetingApplication.Features.MoMAttendances.Commands.Models;
using MeetingApplication.Features.MoMDecisionDrafts.Commands.Models;
using MeetingApplication.Features.MoMDecisionDrafts.Queries.Models;
using MeetingApplication.Features.MoMDecisionDrafts.Queries.Results;
using MeetingApplication.Features.MoMDiscussions.Commands.Models;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingApplication.Features.MoMs.Queries.Models;
using MeetingApplication.Features.MoMs.Queries.Results;
using MeetingApplication.Features.MoMVersions.Queries.Models;
using MeetingApplication.Wrappers; // For PaginatedResult
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/meetings/{meetingId:guid}/mom")] // ✅ المسار المعياري الصحيح
    [Authorize]
    public class MoMController : BaseApiController
    {
        #region 1. Core Lifecycle (Create, Update Content, State Changes)

        // -------------------------------------------------------------
        // CREATE Draft
        // -------------------------------------------------------------
        [HttpPost]
        [Authorize(Policy = "Permission:MoM.Create")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Guid meetingId, [FromBody] CreateMoMCommand command)
        {
            if (meetingId != command.MeetingId) return BadRequest("Meeting ID Mismatch");

            var result = await Mediator.Send(command);

            // نعيد رابط الـ Overview لأنه المورد الرئيسي
            return CreatedSuccess(
                nameof(GetOverview),
                new { meetingId },
                result.Data,
                "MoM Draft Created Successfully");
        }

        // -------------------------------------------------------------
        // UPDATE Content (HTML Only)
        // -------------------------------------------------------------
        [HttpPut("content")]
        [Authorize(Policy = "Permission:MoM.Update")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateContent(Guid meetingId, [FromBody] UpdateMoMContentCommand command)
        {
            if (meetingId != command.MeetingId) return BadRequest("Meeting ID Mismatch");

            await Mediator.Send(command);
            return Success("Content Updated Successfully");
        }

        // -------------------------------------------------------------
        // SUBMIT (Send for Approval)
        // -------------------------------------------------------------
        [HttpPut("submit")]
        [Authorize(Policy = "Permission:MoM.Submit")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Submit(Guid meetingId)
        {
            await Mediator.Send(new SubmitMoMForApprovalCommand(meetingId));
            return Success("Submitted for Approval");
        }

        // -------------------------------------------------------------
        // APPROVE
        // -------------------------------------------------------------
        [HttpPut("approve")]
        [Authorize(Policy = "Permission:MoM.Approve")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Approve(Guid meetingId)
        {
            await Mediator.Send(new ApproveMoMCommand(meetingId));
            return Success("MoM Approved Successfully");
        }

        // -------------------------------------------------------------
        // REJECT
        // -------------------------------------------------------------
        [HttpPut("reject")]
        [Authorize(Policy = "Permission:MoM.Reject")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Reject(Guid meetingId, [FromBody] RejectMoMCommand command)
        {
            if (meetingId != command.MeetingId) return BadRequest("Meeting ID Mismatch");

            await Mediator.Send(command);
            return Success("MoM Rejected");
        }

        // -------------------------------------------------------------
        // ARCHIVE
        // -------------------------------------------------------------
        [HttpPut("archive")]
        [Authorize(Policy = "Permission:MoM.Archive")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Archive(Guid meetingId)
        {
            await Mediator.Send(new ArchiveMoMCommand(meetingId));
            return Success("MoM Archived");
        }

        // -------------------------------------------------------------
        // PUBLISH
        // -------------------------------------------------------------
        [HttpPut("publish")]
        [Authorize(Policy = "Permission:MoM.Publish")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Publish(Guid meetingId)
        {
            await Mediator.Send(new PublishMoMCommand(meetingId));
            return Success("MoM Published");
        }

        #endregion

        #region 2. Decisions Management (Granular CRUD)

        [HttpPost("decisions")]
        [Authorize(Policy = "Permission:MoM.Update")]
        public async Task<IActionResult> AddDecision(Guid meetingId, [FromBody] AddDecisionDraftCommand command)
        {
            if (meetingId != command.MeetingId) return BadRequest("ID Mismatch");
            await Mediator.Send(command);
            return Success("Decision Added");
        }

        [HttpPut("decisions/{decisionId:guid}")]
        [Authorize(Policy = "Permission:MoM.Update")]
        public async Task<IActionResult> UpdateDecision(Guid meetingId, Guid decisionId, [FromBody] UpdateDecisionDraftCommand command)
        {
            if (meetingId != command.MeetingId || decisionId != command.DecisionId) return BadRequest("ID Mismatch");
            await Mediator.Send(command);
            return Success("Decision Updated");
        }

        [HttpDelete("decisions/{decisionId:guid}")]
        [Authorize(Policy = "Permission:MoM.Update")]
        public async Task<IActionResult> RemoveDecision(Guid meetingId, Guid decisionId)
        {
            await Mediator.Send(new RemoveDecisionDraftCommand(meetingId, decisionId));
            return Success("Decision Removed");
        }

        #endregion

        #region 3. Action Items Management (Granular CRUD)

        [HttpPost("action-items")]
        [Authorize(Policy = "Permission:MoM.Update")]
        public async Task<IActionResult> AddActionItem(Guid meetingId, [FromBody] AddActionItemDraftCommand command)
        {
            if (meetingId != command.MeetingId) return BadRequest("ID Mismatch");
            await Mediator.Send(command);
            return Success("Action Item Added");
        }

        [HttpPut("action-items/{actionId:guid}")]
        [Authorize(Policy = "Permission:MoM.Update")]
        public async Task<IActionResult> UpdateActionItem(Guid meetingId, Guid actionId, [FromBody] UpdateActionItemDraftCommand command)
        {
            if (meetingId != command.MeetingId || actionId != command.ActionItemId) return BadRequest("ID Mismatch");
            await Mediator.Send(command);
            return Success("Action Item Updated");
        }

        [HttpDelete("action-items/{actionId:guid}")]
        [Authorize(Policy = "Permission:MoM.Update")]
        public async Task<IActionResult> RemoveActionItem(Guid meetingId, Guid actionId)
        {
            await Mediator.Send(new RemoveActionItemDraftCommand(meetingId, actionId));
            return Success("Action Item Removed");
        }

        #endregion

        #region 4. Attachments Management

        [HttpPost("attachments")]
        [Authorize(Policy = "Permission:MoM.Update")]
        public async Task<IActionResult> AddAttachment(Guid meetingId, [FromBody] AddMoMAttachmentCommand command)
        {
            if (meetingId != command.MeetingId) return BadRequest("ID Mismatch");
            var result = await Mediator.Send(command);
            return Success(result, "Attachment Added");
        }

        [HttpDelete("attachments/{attachmentId:guid}")]
        [Authorize(Policy = "Permission:MoM.Update")]
        public async Task<IActionResult> RemoveAttachment(Guid meetingId, Guid attachmentId)
        {
            await Mediator.Send(new RemoveMoMAttachmentCommand(meetingId, attachmentId));
            return Success("Attachment Removed");
        }

        #endregion

        #region 5. AI Generation

        [HttpPost("generate-ai")]
        [Authorize(Policy = "Permission:MoM.GenerateAI")]
        public async Task<IActionResult> GenerateAI(Guid meetingId, [FromBody] GenerateMoMByAICommand command)
        {
            if (meetingId != command.MeetingId) return BadRequest("ID Mismatch");
            var result = await Mediator.Send(command);
            return Success(result); // Returns HTML string directly
        }

        #endregion

        #region 6. Reads (Queries) - Performance Optimized

        // -------------------------------------------------------------
        // Overview (Main View) - Light & Fast
        // -------------------------------------------------------------
        [HttpGet("overview")]
        [Authorize(Policy = "Permission:MoM.View")]
        [ProducesResponseType(typeof(Result<MoMResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOverview(Guid meetingId)
        {
            var result = await Mediator.Send(new GetMoMByMeetingIdQuery(meetingId));
            return Success(result);
        }

        // -------------------------------------------------------------
        // Decisions List (Paginated Search via POST)
        // -------------------------------------------------------------
        [HttpPost("decisions/search")]
        [Authorize(Policy = "Permission:MoM.View")]
        [ProducesResponseType(typeof(Result<PaginatedResult<MoMDecisionResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDecisions(Guid meetingId, [FromBody] GetMoMDecisionsQuery query)
        {
            query.MeetingId = meetingId; // Securely Enforce ID from Route
            var result = await Mediator.Send(query);
            return Success(result);
        }

        // -------------------------------------------------------------
        // Action Items List (Paginated Search via POST)
        // -------------------------------------------------------------
        [HttpPost("action-items/search")]
        [Authorize(Policy = "Permission:MoM.View")]
        [ProducesResponseType(typeof(Result<PaginatedResult<MoMActionItemResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActionItems(Guid meetingId, [FromBody] GetMoMActionItemsQuery query)
        {
            query.MeetingId = meetingId; // Securely Enforce ID from Route
            var result = await Mediator.Send(query);
            return Success(result);
        }

        // -------------------------------------------------------------
        // Attachments List
        // -------------------------------------------------------------
        [HttpGet("attachments")]
        [Authorize(Policy = "Permission:MoM.View")]
        [ProducesResponseType(typeof(Result<List<MoMAttachmentDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAttachments(Guid meetingId)
        {
            var result = await Mediator.Send(new GetMoMAttachmentsQuery(meetingId));
            return Success(result);
        }

        // -------------------------------------------------------------
        // Versions History
        // -------------------------------------------------------------
        [HttpGet("versions")]
        [Authorize(Policy = "Permission:MoM.View")]
        [ProducesResponseType(typeof(Result<List<MoMVersionSummaryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVersions(Guid meetingId)
        {
            var result = await Mediator.Send(new GetMoMVersionsHistoryQuery(meetingId));
            return Success(result);
        }

        // -------------------------------------------------------------
        // Version Detail (Full Content)
        // -------------------------------------------------------------
        [HttpGet("versions/{versionNumber:int}")]
        [Authorize(Policy = "Permission:MoM.View")]
        [ProducesResponseType(typeof(Result<MoMVersionDetailDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVersionDetail(Guid meetingId, int versionNumber)
        {
            var result = await Mediator.Send(new GetMoMVersionDetailQuery(meetingId, versionNumber));
            return Success(result);
        }

        #endregion

        #region 7. Discussions Management (New)

        // تعديل نص النقاش لبند معين (هذا هو الأكثر استخداماً)
        [HttpPut("discussions/{topicId:guid}")]
        [Authorize(Policy = "Permission:MoMDiscussion.Update")]
        public async Task<IActionResult> UpdateDiscussion(Guid meetingId, Guid topicId, [FromBody] UpdateMoMDiscussionCommand command)
        {
            // تحقق من تطابق المعرفات للأمان
            if (meetingId != command.MeetingId || topicId != command.TopicId)
                return BadRequest("ID Mismatch");

            await Mediator.Send(command);
            return Success("Discussion Topic Updated");
        }

        // إضافة نقاش طارئ (غير موجود في الأجندة الأصلية)
        [HttpPost("discussions")]
        [Authorize(Policy = "Permission:MoMDiscussion.Create")]
        public async Task<IActionResult> AddAdHocDiscussion(Guid meetingId, [FromBody] AddMoMDiscussionCommand command)
        {
            if (meetingId != command.MeetingId) return BadRequest("ID Mismatch");

            var result = await Mediator.Send(command);
            return Success(result, "Ad-hoc Discussion Added");
        }

        // حذف نقاش (مسموح فقط للنقاشات الطارئة عادة، أو حسب قواعد العمل)
        [HttpDelete("discussions/{topicId:guid}")]
        [Authorize(Policy = "Permission:MoMDiscussion.Remove")]
        public async Task<IActionResult> RemoveDiscussion(Guid meetingId, Guid topicId)
        {
            await Mediator.Send(new RemoveMoMDiscussionCommand(meetingId, topicId));
            return Success("Discussion Removed");
        }

        #endregion

        #region 8. Attendance Correction (New)

        // تصحيح حالة الحضور (Correct Snapshot)
        [HttpPut("attendance/{attendanceId:guid}")]
        [Authorize(Policy = "Permission:MoMAttendance.Update")]
        public async Task<IActionResult> CorrectAttendance(Guid meetingId, Guid attendanceId, [FromBody] CorrectMoMAttendanceCommand command)
        {
            // attendanceId هنا هو معرف السطر في جدول MoMAttendance وليس معرف المستخدم
            if (meetingId != command.MeetingId || attendanceId != command.AttendanceRowId)
                return BadRequest("ID Mismatch");

            await Mediator.Send(command);
            return Success("Attendance Record Corrected");
        }

        #endregion
    }
}