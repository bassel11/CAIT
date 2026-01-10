using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApplication.Features.Attachments.Queries.GetAttachment;
using TaskApplication.Features.Attachments.Queries.GetAttachmentHistory;

namespace TaskAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/TaskAttachments")]
    [Authorize]
    public class TaskAttachmentsController : BaseApiController
    {


        // =================================================================
        // حالة خاصة: تحميل الملف
        // هذا الـ Endpoint الوحيد الذي لا يرجع Result<T>
        // =================================================================
        [HttpGet("{taskId}/Download/{attachmentId}")]
        [Authorize(Policy = "Permission:TaskAttachment.Download")]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)] // توثيق أنه يرجع ملف
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)] // توثيق الخطأ
        public async Task<IActionResult> DownloadAttachment(Guid taskId, Guid attachmentId)
        {
            var query = new GetAttachmentContentQuery(taskId, attachmentId);
            var result = await Mediator.Send(query);

            return File(result.Stream, result.ContentType, result.FileName);
        }

        // =================================================================
        // باقي الـ Endpoints تعود للنظام الموحد
        // =================================================================
        [HttpGet("{taskId}/History/{attachmentId}")]
        [Authorize(Policy = "Permission:TaskAttachment.View")]
        public async Task<IActionResult> GetAttachmentVersions(Guid taskId, Guid attachmentId)
        {
            var result = await Mediator.Send(new GetAttachmentHistoryQuery(taskId, attachmentId));

            return Success(result, "AttachmentHistoryRetrievedSuccessfully");
        }
    }
}