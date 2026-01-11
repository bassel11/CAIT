using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Wrappers;
using CommitteeApplication.Features.Committees.Commands.Models;
using CommitteeApplication.Features.Committees.Queries.Models;
using CommitteeApplication.Features.Committees.Queries.Results;
using CommitteeApplication.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommitteeAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Committee")]
    [Authorize]
    public class CommitteeController : BaseApiController
    {
        #region Fields
        private readonly ILogger<CommitteeController> _logger;
        #endregion

        #region Constructor
        public CommitteeController(ILogger<CommitteeController> logger)
        {
            _logger = logger;
        }
        #endregion

        #region Actions

        // -------------------------------------------------------
        // GET By ID
        // -------------------------------------------------------
        [HttpGet("{id}", Name = "GetCommitteeById")]
        [Authorize(Policy = "Permission:Committee.View")]
        [ProducesResponseType(typeof(Result<GetCommitteeByIdResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetCommitteeByIdQuery(id);
            var result = await Mediator.Send(query);
            return Success(result);
        }


        // -------------------------------------------------------
        // CREATE
        // -------------------------------------------------------
        [HttpPost(Name = "CreateCommittee")]
        [Authorize(Policy = "Permission:Committee.Create")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] AddCommitteeCommand command)
        {
            var id = await Mediator.Send(command);
            return CreatedSuccess(
                nameof(GetById),
                new { id = id },
                id,
                "CommitteeCreatedSuccessfully");
        }


        // -------------------------------------------------------
        // UPDATE
        // -------------------------------------------------------
        [HttpPut(Name = "UpdateCommittee")] // يفضل عادة [HttpPut("{id}")] لكن سنبقيها كما طلبت
        [Authorize(Policy = "Permission:Committee.Update")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateCommitteeCommand command)
        {
            var result = await Mediator.Send(command);

            // ✅ استخدام EditSuccess
            return EditSuccess(result, "CommitteeUpdatedSuccessfully");
        }


        // -------------------------------------------------------
        // DELETE
        // -------------------------------------------------------
        [HttpDelete("{id}", Name = "DeleteCommittee")]
        [Authorize(Policy = "Permission:Committee.Delete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var cmd = new DeleteCommitteeCommand() { Id = id };
            await Mediator.Send(cmd);

            // ✅ استخدام Success بدلاً من كائن مجهول
            return Success<string>(null, "CommitteeDeletedSuccessfully");
        }


        // -------------------------------------------------------
        // GET Filtered (Pagination)
        // -------------------------------------------------------
        [HttpPost("filtered", Name = "GetFilteredCommittees")]
        [Authorize(Policy = "Permission:Committee.View")]
        [ProducesResponseType(typeof(Result<PaginatedResult<GetComitsFilteredResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFiltered([FromBody] GetComitsFilteredQuery query)
        {
            var result = await Mediator.Send(query);
            return Success(result);
        }

        #endregion

    }
}
