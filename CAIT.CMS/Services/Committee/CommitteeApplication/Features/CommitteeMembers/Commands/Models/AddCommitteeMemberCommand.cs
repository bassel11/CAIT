using MediatR;

namespace CommitteeApplication.Features.CommitteeMembers.Commands.Models
{
    public class AddCommitteeMemberCommand : IRequest<Guid>
    {
        public Guid CommitteeId { get; set; }
        public Guid UserId { get; set; }                 // Reference to Identity Service
        public string Affiliation { get; set; }          // CAIT department or external entity
        public string ContactDetails { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? LeaveDate { get; set; }
        public bool IsActive { get; set; } = true;

        //العضو يجب أن يكون له دور واحد على الأقل عند الإنشاء
        public List<Guid> RoleIds { get; set; } = new();
    }
}
