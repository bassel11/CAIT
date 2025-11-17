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



            // Mapping جديد للـ filtered response
            CreateMap<Committee, GetComitsFilteredResponse>()
                .ForMember(dest => dest.MembersCount, opt => opt.MapFrom(src => src.CommitteeMembers.Count))
                .ForMember(dest => dest.DocumentsCount, opt => opt.MapFrom(src => src.CommitteeDocuments.Count))
                .ForMember(dest => dest.DecisionsCount, opt => opt.MapFrom(src => src.CommitteeDecisions.Count));


        }
    }
}
