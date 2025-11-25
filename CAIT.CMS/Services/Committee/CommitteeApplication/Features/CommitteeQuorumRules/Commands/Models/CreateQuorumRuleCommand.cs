using CommitteeCore.Enums;
using MediatR;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Commands.Models
{
    public class CreateQuorumRuleCommand : IRequest<Guid>
    {
        public Guid CommitteeId { get; set; }
        public QuorumType Type { get; set; }
        public decimal? ThresholdPercent { get; set; }
        public int? AbsoluteCount { get; set; }
        public bool UsePlusOne { get; set; }
        public string? Description { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
