using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaItems.Commands.Models;
using MeetingApplication.Features.AgendaItems.Queries.Models;
using MeetingApplication.Features.AgendaItems.Queries.Results;
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
        [HttpPost(Name = "CreateAgendaItem")]
        [Authorize(Policy = "Permission:AgendaItem.Create")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] AddAgendaItemCommand command)
        {
            var id = await Mediator.Send(command);

            return Success(id, "AgendaItemCreatedSuccessfully");
        }


        // -------------------------------------------------------
        // UPDATE AgendaItem
        // -------------------------------------------------------
        [HttpPut("{id}", Name = "UpdateAgendaItem")]
        [Authorize(Policy = "Permission:AgendaItem.Update")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateAgendaItemCommand command)
        {
            var updatedId = await Mediator.Send(command);

            // ✅ استخدام EditSuccess أو Success
            return Success(updatedId, "AgendaItemUpdatedSuccessfully");
        }


        // -------------------------------------------------------
        // Delete AgendaItem
        // -------------------------------------------------------
        [HttpDelete("{id}", Name = "DeleteAgendaItem")]
        [Authorize(Policy = "Permission:AgendaItem.Delete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAgendaItem(Guid id)
        {
            await Mediator.Send(new DeleteAgendaItemCommand { Id = id });

            // ✅ استخدام Success بدلاً من الرد بكائن مجهول
            return Success<string>(null, "AgendaItemDeletedSuccessfully");
        }


        // -------------------------------------------------------
        // Generate AgendaItem AI
        // -------------------------------------------------------
        [HttpPost("generate-ai", Name = "GenerateAgendaItem")]
        [Authorize(Policy = "Permission:AgendaItem.Generate")]
        [ProducesResponseType(typeof(Result<List<GetAgendaItemResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateAgendaByAI([FromBody] GenerateAgendaByAICommand command)
        {
            var result = await Mediator.Send(command);

            // ✅ تغليف القائمة بـ Success Result
            return Success(result, "AgendaItemsGeneratedSuccessfully");
        }


        // -------------------------------------------------------
        // Get AgendaItem
        // -------------------------------------------------------
        [HttpGet("GetAgendaItems")]
        [Authorize(Policy = "Permission:AgendaItem.View")]
        [ProducesResponseType(typeof(Result<List<GetAgendaItemResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAgendaItems([FromQuery] Guid meetingId)
        {
            var result = await Mediator.Send(new GetAgendaItemsQuery { MeetingId = meetingId });

            // ✅ تغليف النتيجة
            return Success(result);
        }

        #endregion


    }
}
