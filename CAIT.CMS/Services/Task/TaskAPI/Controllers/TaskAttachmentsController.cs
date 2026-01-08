using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApplication.Features.Attachments.Queries.GetAttachment;
using TaskApplication.Features.Attachments.Queries.GetAttachmentHistory;

namespace TaskAPI.Controllers
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/TaskAttachments")]

    //[Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class TaskAttachmentsController : BaseApiController
    {
        private readonly IMediator _mediator;

        public TaskAttachmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}/Download/{attachmentId}")]
        [Authorize(Policy = "Permission:TaskAttachment.Download")]
        public async Task<IActionResult> DownloadAttachment(Guid id, Guid attachmentId)
        {
            var query = new GetAttachmentContentQuery(id, attachmentId);
            var result = await _mediator.Send(query);

            // إرجاع الملف كـ FileStreamResult
            // المتصفح سيفهم ContentType ويقوم إما بفتحه (PDF/Image) أو تحميله
            return File(result.Stream, result.ContentType, result.FileName);
        }

        [HttpGet("{id}/History/{attachmentId}")]
        [Authorize(Policy = "Permission:TaskAttachment.View")]
        public async Task<IActionResult> GetAttachmentVersions(Guid id, Guid attachmentId)
        {
            var result = await _mediator.Send(new GetAttachmentHistoryQuery(id, attachmentId));
            return Ok(result);
        }

    }
}
