using AutoMapper;
using CommitteeApplication.Commands.Committee;
using CommitteeApplication.Responses;
using CommitteeCore.Entities;

namespace CommitteeApplication.Mappers
{
    public class CommitteeMappingProfile : Profile
    {
        public CommitteeMappingProfile()
        {
            CreateMap<Committee, CommitteeResponse>().ReverseMap();
            CreateMap<AddCommitteeCommand, Committee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id يولد تلقائياً
            CreateMap<UpdateCommitteeCommand, Committee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            //CreateMap<Committee, UpdateCommitteeCommand>().ReverseMap();
        }
    }
}
