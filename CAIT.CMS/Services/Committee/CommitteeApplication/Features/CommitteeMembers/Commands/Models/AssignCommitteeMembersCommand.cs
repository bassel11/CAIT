using CommitteeApplication.Features.CommitteeMembers.Commands.Results;
using MediatR;

namespace CommitteeApplication.Features.CommitteeMembers.Commands.Models
{
    public class AssignCommitteeMembersCommand : IRequest<AssignCommitteeMembersResult>
    {
        public Guid CommitteeId { get; set; }
        public List<AssignCommitteeMemberDto> Members { get; set; } = new();
    }

    public class AssignCommitteeMemberDto
    {
        public Guid UserId { get; set; }
        public string Affiliation { get; set; }
        public string ContactDetails { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? LeaveDate { get; set; }
        public bool IsActive { get; set; } = true;
        // public List<Guid> RoleIds { get; set; } = new();
    }
}
