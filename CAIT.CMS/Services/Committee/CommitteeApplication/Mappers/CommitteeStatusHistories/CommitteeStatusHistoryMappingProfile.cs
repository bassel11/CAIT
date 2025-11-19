using AutoMapper;
using CommitteeApplication.Features.StatusHistories.Queries.Results;
using CommitteeCore.Entities;

namespace CommitteeApplication.Mappers.CommitteeStatusHistories
{
    public class CommitteeStatusHistoryMappingProfile : Profile
    {
        public CommitteeStatusHistoryMappingProfile()
        {

            CreateMap<CommitteeStatusHistory, CommitStatusHistoryResponse>().ReverseMap();

        }

    }
}
