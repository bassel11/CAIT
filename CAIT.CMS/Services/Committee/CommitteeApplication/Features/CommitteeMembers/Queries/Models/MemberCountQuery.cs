using CommitteeApplication.Features.CommitteeMembers.Queries.Results;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CommitteeApplication.Features.CommitteeMembers.Queries.Models
{
    public class MemberCountQuery : IRequest<MemberCountResponse>
    {
        [Required]
        public Guid CommitteeId { get; set; }
        public MemberCountQuery(Guid committeeId)
        {
            CommitteeId = committeeId;
        }
    }
}
