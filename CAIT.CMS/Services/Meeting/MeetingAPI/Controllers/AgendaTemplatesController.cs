using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaTemplates.Commands.Models;
using MeetingApplication.Features.AgendaTemplates.Queries.Models;
using MeetingApplication.Features.AgendaTemplates.Queries.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/AgendaTemplate")]
    //[Authorize] important 
    [Authorize(Roles = "SuperAdmin,CommitteeAdmin")]
    public class AgendaTemplateController : BaseApiController
    {
        #region Fields
        private readonly ILogger<AgendaTemplateController> _logger;
        #endregion

        #region Constructor
        public AgendaTemplateController(ILogger<AgendaTemplateController> logger)
        {
            _logger = logger;
        }
        #endregion

        #region Actions

        // -------------------------------------------------------
        // CREATE Template
        // -------------------------------------------------------
        [HttpPost]
        [Authorize(Policy = "Permission:AgendaTemplate.Create")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateAgendaTemplateCommand command)
        {
            var result = await Mediator.Send(command);
            return Success(result, "Template Created Successfully");

            //return Success(result, "Template Created Successfully", StatusCodes.Status201Created);
        }


        // -------------------------------------------------------
        // UPDATE Template
        // -------------------------------------------------------
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "Permission:AgendaTemplate.Update")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAgendaTemplateCommand command)
        {
            if (id != command.Id)
                return BadRequest(Result.Failure("ID mismatch"));

            var result = await Mediator.Send(command);
            return Success(result, "Template Updated Successfully");
        }

        // -------------------------------------------------------
        // DELETE Template
        // -------------------------------------------------------
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "Permission:AgendaTemplate.Delete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await Mediator.Send(new DeleteAgendaTemplateCommand(id));
            return Success(result, "Template Deleted Successfully");
        }


        // -------------------------------------------------------
        // GET Template By Id
        // -------------------------------------------------------
        [HttpGet("{id:guid}")]
        [Authorize(Policy = "Permission:AgendaTemplate.View")]
        [ProducesResponseType(typeof(Result<AgendaTemplateResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await Mediator.Send(new GetAgendaTemplateByIdQuery(id));
            return Success(result);
        }


        // -------------------------------------------------------
        // GET All Templates
        // -------------------------------------------------------
        [HttpGet]
        [Authorize(Policy = "Permission:AgendaTemplate.View")]
        [ProducesResponseType(typeof(Result<List<AgendaTemplateSummaryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new GetAllAgendaTemplatesQuery());
            return Success(result);
        }


        // -------------------------------------------------------
        // SEARCH / GET All (Optional - For Listing in UI)
        // -------------------------------------------------------
        // [HttpPost("search")]
        // [Authorize(Policy = "Permission:AgendaTemplate.View")]
        // public async Task<IActionResult> Search([FromBody] GetAgendaTemplatesQuery query) 
        // {
        //     var result = await Mediator.Send(query);
        //     return Success(result);
        // }

        #endregion
    }
}
