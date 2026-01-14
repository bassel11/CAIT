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
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.StatusId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());
            //CreateMap<Committee, UpdateCommitteeCommand>().ReverseMap();

            CreateMap<CommitteeStatus, CommitteeStatusDto>();
            CreateMap<Committee, GetComitsFilteredResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));


        }
    }
}
