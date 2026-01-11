using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Wrappers;
using CommitteeApplication.Features.CommitteeStatuses.Queries.Models;
using CommitteeApplication.Features.CommitteeStatuses.Queries.Results;
using CommitteeApplication.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommitteeAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/CommitteeStatus")]
    [Authorize]
    public class CommitteeStatusController : BaseApiController
    {


        #region Constructor
        public CommitteeStatusController()
        {
        }
        #endregion

        #region Actions

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet(Name = "GetAllCommitteeStatuses")]
        [Authorize(Policy = "Permission:CommitteeStatus.View")]
        [ProducesResponseType(typeof(Result<IEnumerable<GetCommitteeStatusResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetCommitteeStatusesQuery();
            var data = await Mediator.Send(query);
            return Success(data);
        }

        // -------------------------------------------------------
        // GET Filtered (Pagination)
        // -------------------------------------------------------
        [HttpPost("filtered", Name = "GetFilteredCommitteeStatuses")]
        [Authorize(Policy = "Permission:CommitteeStatus.View")]
        [ProducesResponseType(typeof(Result<PaginatedResult<GetCommitteeStatusResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFiltered([FromBody] GetCommitStatFilterdQuery query)
        {
            var result = await Mediator.Send(query);
            return Success(result);
        }

        #endregion
    }
}
