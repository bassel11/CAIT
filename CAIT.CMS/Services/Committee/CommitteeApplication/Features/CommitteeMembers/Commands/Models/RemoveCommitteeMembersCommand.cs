using CommitteeApplication.Features.CommitteeMembers.Commands.Results;
using MediatR;

namespace CommitteeApplication.Features.CommitteeMembers.Commands.Models
{
    public class RemoveCommitteeMembersCommand : IRequest<RemoveCommitteeMembersResult>
    {
        public Guid CommitteeId { get; set; }
        public List<Guid> MembersIds { get; set; } = new();
    }
}
