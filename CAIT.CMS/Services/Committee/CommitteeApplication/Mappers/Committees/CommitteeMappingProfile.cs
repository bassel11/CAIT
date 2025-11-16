using AutoMapper;
using CommitteeApplication.Features.Committees.Commands.Models;
using CommitteeApplication.Features.Committees.Queries.Results;
using CommitteeCore.Entities;

namespace CommitteeApplication.Mappers.Committees
{
    public class CommitteeMappingProfile : Profile
    {
        public CommitteeMappingProfile()
        {
            CreateMap<Committee, GetCommitteeByIdResponse>().ReverseMap();
            CreateMap<AddCommitteeCommand, Committee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id يولد تلقائياً
            CreateMap<UpdateCommitteeCommand, Committee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            //CreateMap<Committee, UpdateCommitteeCommand>().ReverseMap();
        }
    }
}
