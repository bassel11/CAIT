using MediatR;

namespace CommitteeApplication.Features.CommitteeMembers.Commands.Models
{
    public class AddCommitteeMemberCommand : IRequest<Guid>
    {
        public Guid CommitteeId { get; set; }
        public Guid UserId { get; set; }                 // Reference to User Service
        public string Role { get; set; }                 // Chairman, Vice Chairman, Rapporteur, Member, Observer
        public string Affiliation { get; set; }          // CAIT department or external entity
        public string ContactDetails { get; set; }
    }
}
