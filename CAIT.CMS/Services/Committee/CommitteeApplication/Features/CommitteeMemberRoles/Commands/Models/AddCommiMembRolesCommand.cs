using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Results;
using MediatR;

namespace CommitteeApplication.Features.CommitteeMemberRoles.Commands.Models
{
    public class AddCommiMembRolesCommand : IRequest<AddCommiMembRolesResult>
    {
        public Guid CommitteeMemberId { get; set; }
        //   public Guid CommitteeId { get; set; }
        public List<Guid> RoleIds { get; set; } = new();
    }
}
