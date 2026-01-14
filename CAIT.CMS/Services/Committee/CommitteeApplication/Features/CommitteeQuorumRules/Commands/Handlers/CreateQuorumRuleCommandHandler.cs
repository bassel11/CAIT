using AutoMapper;
using CommitteeApplication.Features.CommitteeQuorumRules.Commands.Models;
using CommitteeCore.Entities;
using CommitteeCore.Enums;
using CommitteeCore.Repositories;
using MediatR;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Commands.Handlers
{
    public class CreateQuorumRuleCommandHandler : IRequestHandler<CreateQuorumRuleCommand, Guid>
    {
        private readonly ICommitteeQuorumRuleRepository _repo;
        private readonly IMapper _mapper;

        public CreateQuorumRuleCommandHandler(ICommitteeQuorumRuleRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateQuorumRuleCommand cmd, CancellationToken ct)
        {
            if (await _repo.ExistsByCommitteeId(cmd.CommitteeId))
                throw new InvalidOperationException("Committee already has a quorum rule.");

            //var rule = new CommitteeQuorumRule
            //{
            //    CommitteeId = cmd.CommitteeId,
            //    Type = cmd.Type,
            //    ThresholdPercent = cmd.ThresholdPercent,
            //    AbsoluteCount = cmd.AbsoluteCount,
            //    UsePlusOne = cmd.UsePlusOne,
            //    Description = cmd.Description,
            //    EffectiveDate = cmd.EffectiveDate,
            //    ExpiryDate = cmd.ExpiryDate
            //};

            var rule = _mapper.Map<CommitteeQuorumRule>(cmd);


            if (rule.Type == QuorumType.AbsoluteNumber)
            {
                rule.ThresholdPercent = null;
                rule.UsePlusOne = false; // عادة لا تستخدم مع العدد الثابت
            }
            else // Percentage OR PercentagePlusOne
            {
                rule.AbsoluteCount = null;
            }

            // 4. ضبط بيانات النظام
            rule.Id = Guid.NewGuid();
            rule.CreatedAt = DateTime.UtcNow;

            // 5. الحفظ
            await _repo.AddAsync(rule);

            return rule.Id;
        }
    }
}
