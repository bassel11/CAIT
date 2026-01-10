using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Pagination;
using BuildingBlocks.Shared.Wrappers;
using DecisionApplication.Decisions.Commands.CreateDecision;
using DecisionApplication.Decisions.Commands.DeleteDecision;
using DecisionApplication.Decisions.Commands.UpdateDecision;
using DecisionApplication.Decisions.Queries.GetDecisionById;
using DecisionApplication.Decisions.Queries.GetDecisions;
using DecisionApplication.Decisions.Queries.GetDecisionsByMeeting;
using DecisionApplication.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DecisionAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Decisions")]
    [Authorize]
    public class DecisionsController : BaseApiController
    {

        // 1. Get By Id
        [HttpGet("{id:guid}")]
        [Authorize(Policy = "Permission:Decision.View")]
        [ProducesResponseType(typeof(Result<DecisionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await Mediator.Send(new GetDecisionByIdQuery(id));
            // لا حاجة لـ if (result == null) لأن الـ Handler سيرمي NotFoundException
            return Success(result, "DataRetrievedSuccessfully");
        }

        // 2. Create Decision
        [HttpPost]
        [Authorize(Policy = "Permission:Decision.Create")]
        [ProducesResponseType(typeof(Result<CreateDecisionResult>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateDecisionDto createDecisionDto)
        {
            var command = new CreateDecisionCommand(createDecisionDto);
            var result = await Mediator.Send(command);

            return CreatedSuccess(
                nameof(GetById),
                new { id = result.Id, version = "1.0" },
                result,
                "DecisionCreatedSuccessfully"
            );
        }
        // 3. Update Decision
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "Permission:Decision.Update")]
        [ProducesResponseType(typeof(Result<UpdateDecisionResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDecisionDto updateDecisionDto)
        {
            // تأكد من تمرير الـ ID من الرابط للـ Command لضمان الأمان
            var command = new UpdateDecisionCommand(id, updateDecisionDto);
            var result = await Mediator.Send(command);

            return Success(result, "DecisionUpdatedSuccessfully");
        }

        // 4. Delete Decision
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "Permission:Decision.Delete")]
        [ProducesResponseType(typeof(Result<DeleteDecisionResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteDecisionCommand(id);
            var result = await Mediator.Send(command);

            return Success(result, "DecisionDeletedSuccessfully");
        }

        // 5. Get Paginated Decisions
        [HttpGet]
        [Authorize(Policy = "Permission:Decision.View")]
        [ProducesResponseType(typeof(Result<PaginatedResult<DecisionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            // ملاحظة: يفضل أن يبدأ pageIndex من 1 وليس 0 في الـ Query String
            var query = new GetDecisionsQuery(new PaginationRequest(pageIndex, pageSize));
            var result = await Mediator.Send(query);

            return Success(result, "DataRetrievedSuccessfully");
        }

        // 6. Get Decisions By Meeting
        [HttpGet("meeting/{meetingId:guid}")]
        [Authorize(Policy = "Permission:Decision.View")]
        [ProducesResponseType(typeof(Result<List<DecisionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByMeeting(Guid meetingId)
        {
            var query = new GetDecisionsByMeetingQuery(meetingId);
            var result = await Mediator.Send(query);

            return Success(result, "DataRetrievedSuccessfully");
        }

        // 7. Add Vote (Placeholder)
        [HttpPost("{id:guid}/votes")]
        [Authorize(Policy = "Permission:Vote.Create")]
        public async Task<IActionResult> AddVote(Guid id, [FromBody] VoteDto voteDto)
        {
            // عند تطبيق التصويت، استخدم Success() أيضاً
            // var command = new AddVoteCommand(id, voteDto);
            // await Mediator.Send(command);
            // return Success(_localizer["VoteAddedSuccessfully"]);

            // حالياً نرمي استثناء "غير مطبق" بشكل قياسي
            throw new NotImplementedException("Voting feature is coming soon.");
        }

    }
}
