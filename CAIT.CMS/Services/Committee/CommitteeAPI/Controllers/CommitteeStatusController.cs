using CommitteeApplication.Features.CommitteeStatuses.Queries.Models;
using CommitteeApplication.Features.CommitteeStatuses.Queries.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CommitteeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommitteeStatusController : ControllerBase
    {
        #region Fields
        private readonly IMediator _mediator;
        #endregion

        #region Constructor
        public CommitteeStatusController(IMediator mediator)
        {
            _mediator = mediator;
        }
        #endregion

        #region Actions

        [HttpGet(Name = "GetAllCommitteeStatuses")]
        [ProducesResponseType(typeof(IEnumerable<GetCommitteeStatusResponse>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:CommitteeStatus.View")]
        public async Task<ActionResult<IEnumerable<GetCommitteeStatusResponse>>> GetAll()
        {
            var query = new GetCommitteeStatusesQuery();
            var data = await _mediator.Send(query);
            return Ok(data);
        }

        #endregion
    }
}
