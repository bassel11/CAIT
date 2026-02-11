using Asp.Versioning;
using BuildingBlocks.Shared.Controllers;
using BuildingBlocks.Shared.Wrappers;
using CommitteeApplication.Features.CommitteeMembers.Commands.Models;
using CommitteeApplication.Features.CommitteeMembers.Commands.Results;
using CommitteeApplication.Features.CommitteeMembers.Queries.Models;
using CommitteeApplication.Features.CommitteeMembers.Queries.Results;
using CommitteeApplication.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommitteeAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/CommitteeMember")]
    [Authorize]
    public class CommitteeMemberController : BaseApiController
    {
        #region Fields
        private readonly ILogger<CommitteeMemberController> _logger;
        #endregion

        #region Constructor
        public CommitteeMemberController(ILogger<CommitteeMemberController> logger)
        {
            _logger = logger;
        }
        #endregion

        #region Actions

        // -------------------------------------------------------
        // GET List By CommitteeId
        // -------------------------------------------------------
        [HttpGet("{committeeId}", Name = "GetCommitteeMembersByCommitteeId")]
        [Authorize(Policy = "Permission:CommitteeMember.View")]
        [ProducesResponseType(typeof(Result<IEnumerable<CommitteeMemberResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCommitteeId(Guid committeeId)
        {
            var query = new GetComMembersListQuery(committeeId);
            var result = await Mediator.Send(query);

            return Success(result);
        }

        // -------------------------------------------------------
        // CREATE (Single)
        // -------------------------------------------------------
        [HttpPost(Name = "CreateCommitteeMember")]
        [Authorize(Policy = "Permission:CommitteeMember.Create")]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)] // استخدمنا 200 لعدم وجود GetById
        public async Task<IActionResult> Create([FromBody] AddCommitteeMemberCommand command)
        {
            var result = await Mediator.Send(command);

            // ⚠️ ملاحظة: نستخدم Success بدلاً من CreatedSuccess 
            // لأنه لا توجد دالة "GetMemberById" لنقوم بالتوجيه إليها في الـ Header.
            return Success(result, "CommitteeMemberCreatedSuccessfully");
        }

        // -------------------------------------------------------
        // UPDATE
        // -------------------------------------------------------
        [HttpPut(Name = "UpdateCommitteeMember")]
        [Authorize(Policy = "Permission:CommitteeMember.Update")]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateCommitteeMemberCommand command)
        {
            var result = await Mediator.Send(command);

            // ✅ استخدام EditSuccess المضافة حديثاً
            return EditSuccess(result, "CommitteeMemberUpdatedSuccessfully");
        }

        // -------------------------------------------------------
        // DELETE
        // -------------------------------------------------------
        [HttpDelete("{id}", Name = "DeleteCommitteeMember")]
        [Authorize(Policy = "Permission:CommitteeMember.Delete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var cmd = new DeleteCommitteeMemberCommand() { Id = id };
            await Mediator.Send(cmd);

            return Success("CommitteeMemberDeletedSuccessfully");
        }


        // -------------------------------------------------------
        // ASSIGN Multiple Members
        // -------------------------------------------------------
        [HttpPost("assign-multiple", Name = "AssignCommitteeMembers")]
        [Authorize(Policy = "Permission:CommitteeMember.Create")]
        [ProducesResponseType(typeof(Result<AssignCommitteeMembersResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AssignMultiple([FromBody] AssignCommitteeMembersCommand command)
        {
            var result = await Mediator.Send(command);

            // يمكن التحقق هنا إذا كانت القائمة فارغة لإرجاع رسالة مختلفة، 
            // لكن يفضل ترك المنطق للـ Handler وإرجاع النتيجة كما هي
            return Success(result, "MembersAssignedSuccessfully");
        }

        // -------------------------------------------------------
        // REMOVE Multiple Members
        // -------------------------------------------------------
        [HttpPost("remove-multiple", Name = "RemoveCommitteeMembers")]
        [Authorize(Policy = "Permission:CommitteeMember.Delete")]
        [ProducesResponseType(typeof(Result<RemoveCommitteeMembersResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveMultiple([FromBody] RemoveCommitteeMembersCommand command)
        {
            var result = await Mediator.Send(command);
            return Success(result, "MembersRemovedSuccessfully");
        }

        // -------------------------------------------------------
        // GET Filtered (Pagination)
        // -------------------------------------------------------
        [HttpPost("filtered", Name = "GetFilteredCommitteeMembers")]
        [Authorize(Policy = "Permission:Committee.View")]
        [ProducesResponseType(typeof(Result<PaginatedResult<CommitMembsFilterResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFiltered([FromBody] GetComitMembsFilteredQuery query)
        {
            var result = await Mediator.Send(query);
            return Success(result);
        }

        // -------------------------------------------------------
        // GET Count
        // -------------------------------------------------------
        [HttpGet("{committeeId:guid}/count", Name = "GetMemberCount")]
        [Authorize(Policy = "Permission:CommitteeMember.View")]
        [ProducesResponseType(typeof(Result<MemberCountResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCount(Guid committeeId, CancellationToken ct)
        {
            var result = await Mediator.Send(new MemberCountQuery(committeeId), ct);
            return Success(result);
        }


        // -------------------------------------------------------
        // GET Integration Members (For Meeting Service)
        // -------------------------------------------------------
        [HttpGet("GetCommitteeMembers/{committeeId}")] // يطابق المسار الذي وضعناه في Client
        //[Authorize] // قد تحتاج لسياسة خاصة مثل "ServiceToService"
        [ProducesResponseType(typeof(List<CommitteeMemberIntegrationResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMembersForIntegration(Guid committeeId)
        {
            var query = new GetCommitteeMembersForIntegrationQuery(committeeId);
            var result = await Mediator.Send(query);
            return Ok(result); // نرجع القائمة مباشرة لتسهيل الـ Deserialize في الطرف الآخر
        }


        #endregion
    }
}
