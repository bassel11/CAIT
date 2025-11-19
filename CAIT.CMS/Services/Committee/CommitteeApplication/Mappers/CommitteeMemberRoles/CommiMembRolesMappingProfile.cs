using AutoMapper;
using CommitteeApplication.Features.CommitteeMemberRoles.Queries.Results;
using CommitteeCore.Entities;

namespace CommitteeApplication.Mappers.CommitteeMemberRoles
{
    public class CommiMembRolesMappingProfile : Profile
    {
        public CommiMembRolesMappingProfile()
        {
            CreateMap<CommitteeMemberRole, GetCommiMembRolesResponse>().ReverseMap();
        }
    }
}
