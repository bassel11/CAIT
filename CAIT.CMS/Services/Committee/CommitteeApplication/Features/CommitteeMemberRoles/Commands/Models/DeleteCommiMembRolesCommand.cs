using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Results;
using MediatR;

namespace CommitteeApplication.Features.CommitteeMemberRoles.Commands.Models
{
    public class DeleteCommiMembRolesCommand : IRequest<DeleteCommiMembRolesResult>
    {
        public Guid Id { get; set; }   // CommitteeMemberRole Id
    }
}
