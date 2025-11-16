using MediatR;

namespace CommitteeApplication.Features.CommitteeMembers.Commands.Models
{
    public class DeleteCommitteeMemberCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
