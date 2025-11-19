using AutoMapper;
using CommitteeApplication.Features.CommitteeStatuses.Queries.Results;
using CommitteeCore.Entities;

namespace CommitteeApplication.Mappers.CommitteeStatuses
{
    public class CommitteeStatusesMappingProfile : Profile
    {
        public CommitteeStatusesMappingProfile()
        {
            CreateMap<CommitteeStatus, GetCommitteeStatusResponse>();
        }
    }
}
