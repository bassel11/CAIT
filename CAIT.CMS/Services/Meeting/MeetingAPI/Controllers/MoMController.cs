using MediatR;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingApplication.Features.MoMs.Queries.Models;
using MeetingApplication.Features.MoMs.Queries.Results;
using MeetingApplication.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MeetingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MoMController : ControllerBase
    {
        #region Fields
        private readonly IMediator _mediator;
        private readonly ILogger<MoMController> _logger;
        #endregion

        #region Constructor
        public MoMController(IMediator mediator
                                   , ILogger<MoMController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        #endregion

        #region Actions

        // -------------------------------------------------------------
        // POST: api/MoM/Create
        // -------------------------------------------------------------
        [HttpPost("create")]
        [Authorize(Policy = "Permission:MoM.Create")]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateMoMCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        // -------------------------------------------------------------
        // PUT: api/MoM/Update
        // -------------------------------------------------------------
        [HttpPut("update")]
        [Authorize(Policy = "Permission:MoM.Update")]
        public async Task<ActionResult<Guid>> Update([FromBody] UpdateMoMCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }


        // -------------------------------------------------------------
        // PUT: api/MoM/Submit
        // -------------------------------------------------------------
        [HttpPut("submit")]
        [Authorize(Policy = "Permission:MoM.Submit")]
        public async Task<ActionResult<Guid>> Approve([FromBody] SubmitMoMForApprovalCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok();
        }


        // -------------------------------------------------------------
        // PUT: api/MoM/Approve
        // -------------------------------------------------------------
        [HttpPut("approve")]
        [Authorize(Policy = "Permission:MoM.Approve")]
        public async Task<ActionResult<Guid>> Approve([FromBody] ApproveMoMCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok();
        }

        // -------------------------------------------------------------
        // PUT: api/MoM/Reject
        // -------------------------------------------------------------
        [HttpPut("reject")]
        [Authorize(Policy = "Permission:MoM.Reject")]
        public async Task<ActionResult<Guid>> Reject([FromBody] RejectMoMCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        // -------------------------------------------------------------
        // GET: api/MoM/GetById
        // -------------------------------------------------------------
        [HttpGet("GetById/{momId}")]
        [Authorize(Policy = "Permission:MoM.View")]
        public async Task<ActionResult<GetMinutesResponse>> GetById(Guid momId, CancellationToken ct)
        {
            var query = new GetMoMByIdQuery { MoMId = momId };
            var result = await _mediator.Send(query, ct);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // -------------------------------------------------------------
        // GET: api/MoM/GetByMeetingId
        // -------------------------------------------------------------
        [HttpGet("GetByMeetingId/{meetingId}")]
        [Authorize(Policy = "Permission:MoM.View")]
        public async Task<ActionResult<List<GetMinutesResponse>>> GetByMeetingId(Guid meetingId, CancellationToken ct)
        {
            var query = new GetMoMsForMeetingQuery { MeetingId = meetingId };
            var result = await _mediator.Send(query, ct);

            if (result == null || result.Count == 0)
                return NotFound(); // 404 إذا لم يوجد أي MoMs

            return Ok(result); // 200 مع القائمة
        }


        // -------------------------------------------------------
        // Get Paginated and filtered and Sorted MoMs
        // -------------------------------------------------------
        [HttpPost("GetMoMs", Name = "GetMoMs")]
        [ProducesResponseType(typeof(PaginatedResult<GetMinutesResponse>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "Permission:MoM.View")]
        public async Task<ActionResult<PaginatedResult<GetMinutesResponse>>> GetMeetings([FromBody] GetMoMsByMeetingQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        #endregion
    }
}
