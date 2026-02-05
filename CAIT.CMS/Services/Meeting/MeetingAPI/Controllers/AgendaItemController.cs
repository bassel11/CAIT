using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaItems.Commands.Models;
using MeetingApplication.Features.AgendaItems.Queries.Models;
using MeetingApplication.Features.AgendaItems.Queries.Results;
using MeetingApplication.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/AgendaItem")]

    //[Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class AgendaItemController : BaseApiController
    {
        #region Fields
        private readonly ILogger<AgendaItemController> _logger;
        #endregion

        #region Constructor
        public AgendaItemController(ILogger<AgendaItemController> logger)
        {
            _logger = logger;
        }
        #endregion

        #region Actions

        // -------------------------------------------------------
        // CREATE AgendaItem
        // -------------------------------------------------------
        [HttpPost()]
        [Authorize(Policy = "Permission:AgendaItem.Create")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] AddAgendaItemCommand command)
        {
            var result = await Mediator.Send(command);
            return Success(result, "AgendaItemCreatedSuccessfully");
        }


        // -------------------------------------------------------
        // UPDATE AgendaItem
        // -------------------------------------------------------
        [HttpPut("{id:guid}", Name = "UpdateAgendaItem")]
        [Authorize(Policy = "Permission:AgendaItem.Update")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAgendaItemCommand command)
        {
            if (id != command.AgendaItemId)
                return BadRequest("ID mismatch");

            var result = await Mediator.Send(command);
            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }


        // -------------------------------------------------------
        // Delete AgendaItem
        // -------------------------------------------------------
        [HttpDelete("{meetingId:guid}/{id:guid}")] // نحتاج MeetingId للوصول للـ Aggregate
        [Authorize(Policy = "Permission:AgendaItem.Delete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid meetingId, Guid id)
        {
            var result = await Mediator.Send(new DeleteAgendaItemCommand(meetingId, id));
            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }


        // -------------------------------------------------------
        // Generate Agenda Suggestions (AI)
        // -------------------------------------------------------
        [HttpPost("generate-ai")]
        [Authorize(Policy = "Permission:AgendaItem.Generate")]
        [ProducesResponseType(typeof(Result<List<AgendaItemResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateAgendaByAI([FromBody] GenerateAgendaByAICommand command)
        {
            var result = await Mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }


        // -------------------------------------------------------
        // GET (Paginated)
        // -------------------------------------------------------
        [HttpPost("search")] // Post for complex search criteria
        [Authorize(Policy = "Permission:AgendaItem.View")]
        [ProducesResponseType(typeof(Result<PaginatedResult<AgendaItemResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAgendaItems([FromBody] GetAgendaItemsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        // -------------------------------------------------------
        // Load Template
        // -------------------------------------------------------
        [HttpPost("load-template")]
        [Authorize(Policy = "Permission:AgendaItem.Create")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoadTemplate([FromBody] LoadAgendaTemplateCommand command)
        {
            if (command.MeetingId == Guid.Empty || command.TemplateId == Guid.Empty)
                return BadRequest(Result.Failure("Invalid MeetingId or TemplateId."));

            var result = await Mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        // -------------------------------------------------------
        // Add Attachment
        // -------------------------------------------------------
        [HttpPost("{agendaItemId:guid}/attachments")]
        [Authorize(Policy = "Permission:AgendaItemAttachment.Upload")] // إضافة مرفق يعتبر تعديل للبند
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddAttachment(Guid agendaItemId, [FromBody] AddAgendaItemAttachmentCommand command)
        {
            if (agendaItemId != command.AgendaItemId)
                return BadRequest(Result.Failure("Agenda Item ID mismatch."));

            var result = await Mediator.Send(command);

            if (result.Succeeded)
                return StatusCode(StatusCodes.Status201Created, result);

            return BadRequest(result);
        }

        // -------------------------------------------------------
        // Remove Attachment
        // -------------------------------------------------------
        [HttpDelete("{agendaItemId:guid}/attachments/{attachmentId:guid}")]
        [Authorize(Policy = "Permission:AgendaItemAttachment.Remove")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveAttachment(Guid agendaItemId, Guid attachmentId, [FromQuery] Guid meetingId)
        {
            var command = new RemoveAgendaItemAttachmentCommand(meetingId, agendaItemId, attachmentId);
            var result = await Mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }


        #endregion


    }
}
