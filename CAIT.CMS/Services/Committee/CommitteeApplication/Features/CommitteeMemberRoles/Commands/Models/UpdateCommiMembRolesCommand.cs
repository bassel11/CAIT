using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Results;
using MediatR;

namespace CommitteeApplication.Features.CommitteeMemberRoles.Commands.Models
{
    public class UpdateCommiMembRolesCommand : IRequest<UpdateCommiMembRolesResult>
    {
        public Guid Id { get; set; }                   // CommitteeMemberRole Id
        public Guid RoleId { get; set; }               // New RoleId
    }
}
