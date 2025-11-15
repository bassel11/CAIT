using MediatR;

namespace CommitteeApplication.Commands.CommitteeMembers
{
    public class DeleteCommitteeMemberCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
