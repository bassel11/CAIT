using AutoMapper;
using CommitteeApplication.Features.CommitteeQuorumRules.Commands.Models;
using CommitteeApplication.Features.CommitteeQuorumRules.Queries.Results;
using CommitteeCore.Entities;

namespace CommitteeApplication.Mappers.CommitteeQuorumRules
{
    public class QuorumRuleMappingProfile : Profile
    {
        public QuorumRuleMappingProfile()
        {
            CreateMap<CommitteeQuorumRule, CommitteeQuorumRuleResponse>().ReverseMap();

            CreateMap<CreateQuorumRuleCommand, CommitteeQuorumRule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id يولد تلقائياً
            CreateMap<UpdateQuorumRuleCommand, CommitteeQuorumRule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
