using MediatR;
using MeetingApplication.Features.AgendaItems.Commands.Models;
using MeetingApplication.Features.AgendaItems.Queries.Models;
using MeetingApplication.Features.AgendaItems.Queries.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MeetingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AgendaItemController : ControllerBase
    {
        #region Fields
        private readonly IMediator _mediator;
        private readonly ILogger<AgendaItemController> _logger;
        #endregion

        #region Constructor
        public AgendaItemController(IMediator mediator
                                   , ILogger<AgendaItemController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        #endregion

        #region Actions

        // -------------------------------------------------------
        // CREATE AgendaItem
        // -------------------------------------------------------
        [HttpPost(Name = "CreateAgendaItem")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:AgendaItem.Create")]
        public async Task<ActionResult<Guid>> Create([FromBody] AddAgendaItemCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }


        // -------------------------------------------------------
        // UPDATE AgendaItem
        // -------------------------------------------------------
        [HttpPut("{id}", Name = "UpdateAgendaItem")]
        [Authorize(Policy = "Permission:AgendaItem.Update")]
        public async Task<ActionResult<Guid>> Update([FromBody] UpdateAgendaItemCommand command)
        {
            var updatedId = await _mediator.Send(command);
            return Ok(updatedId);
        }


        // -------------------------------------------------------
        // Delete AgendaItem
        // -------------------------------------------------------
        [HttpDelete("{id}", Name = "DeleteAgendaItem")]
        [Authorize(Policy = "Permission:AgendaItem.Delete")]
        public async Task<IActionResult> DeleteAgendaItem(Guid id)
        {

            await _mediator.Send(new DeleteAgendaItemCommand { Id = id });
            return Ok(new
            {
                message = "AgendaItem Deleted successfully"
            });
        }


        // -------------------------------------------------------
        // Generate AgendaItem
        // -------------------------------------------------------
        [HttpPost("generate-ai", Name = "GenerateAgendaItem")]
        [Authorize(Policy = "Permission:AgendaItem.Generate")]
        public async Task<ActionResult<List<GetAgendaItemResponse>>> GenerateAgendaByAI(
            [FromBody] GenerateAgendaByAICommand command)
        {

            var result = await _mediator.Send(command);
            return Ok(result);
        }


        // -------------------------------------------------------
        // Get AgendaItem
        // -------------------------------------------------------
        [HttpGet("GetAgendaItems")]
        [Authorize(Policy = "Permission:AgendaItem.View")]
        public async Task<ActionResult<List<GetAgendaItemResponse>>> GetAgendaItems([FromQuery] Guid meetingId)
        {
            var result = await _mediator.Send(new GetAgendaItemsQuery { MeetingId = meetingId });
            return Ok(result);
        }

        #endregion


    }
}
