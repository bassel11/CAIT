using AutoMapper;
using CommitteeApplication.Features.CommitteeQuorumRules.Commands.Models;
using CommitteeCore.Repositories;
using MediatR;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Commands.Handlers
{
    public class UpdateQuorumRuleCommandHandler : IRequestHandler<UpdateQuorumRuleCommand, bool>
    {
        private readonly ICommitteeQuorumRuleRepository _repo;
        private readonly IMapper _mapper;

        public UpdateQuorumRuleCommandHandler(ICommitteeQuorumRuleRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateQuorumRuleCommand cmd, CancellationToken ct)
        {
            var rule = await _repo.GetByIdAsync(cmd.Id);
            if (rule == null) return false;

            //rule.Type = cmd.Type;
            //rule.ThresholdPercent = cmd.ThresholdPercent;
            //rule.AbsoluteCount = cmd.AbsoluteCount;
            //rule.UsePlusOne = cmd.UsePlusOne;
            //rule.Description = cmd.Description;
            //rule.EffectiveDate = cmd.EffectiveDate;
            //rule.ExpiryDate = cmd.ExpiryDate;

            // تطبيق التحديث باستخدام AutoMapper
            _mapper.Map(cmd, rule);

            await _repo.UpdateAsync(rule);

            return true;
        }
    }
}
