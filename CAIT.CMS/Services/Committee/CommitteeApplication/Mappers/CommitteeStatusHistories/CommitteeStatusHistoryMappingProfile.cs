using AutoMapper;
using CommitteeApplication.Features.StatusHistories.Commands.Models;
using CommitteeApplication.Features.StatusHistories.Queries.Results;
using CommitteeCore.Entities;

namespace CommitteeApplication.Mappers.CommitteeStatusHistories
{
    public class CommitteeStatusHistoryMappingProfile : Profile
    {
        public CommitteeStatusHistoryMappingProfile()
        {

            CreateMap<CommitteeStatusHistory, CommitStatusHistoryResponse>()
            .ForMember(dest => dest.OldStatusName,
                       opt => opt.MapFrom(src => src.OldStatus.Name))
            .ForMember(dest => dest.NewStatusName,
                       opt => opt.MapFrom(src => src.NewStatus.Name));

            CreateMap<AddCommitStatusHistoryCommand, CommitteeStatusHistory>();
        }

    }
}
