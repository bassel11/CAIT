using AutoMapper;
using CommitteeApplication.Responses;
using CommitteeCore.Entities;

namespace CommitteeApplication.Mappers
{
    public class ComMemberMappingProfile : Profile
    {
        public ComMemberMappingProfile()
        {
            CreateMap<CommitteeMember, CommitteeMemberResponse>().ReverseMap();
        }
    }
}
