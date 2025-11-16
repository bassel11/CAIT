using AutoMapper;
using CommitteeApplication.Features.CommitteeMembers.Commands.Models;
using CommitteeApplication.Features.CommitteeMembers.Queries.Results;
using CommitteeCore.Entities;

namespace CommitteeApplication.Mappers.CommitteeMembers
{
    public class ComMemberMappingProfile : Profile
    {
        public ComMemberMappingProfile()
        {
            CreateMap<CommitteeMember, CommitteeMemberResponse>().ReverseMap();
            CreateMap<AddCommitteeMemberCommand, CommitteeMember>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id يولد تلقائياً
            CreateMap<UpdateCommitteeMemberCommand, CommitteeMember>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
