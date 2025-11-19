using AutoMapper;
using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Models;
using CommitteeApplication.Features.CommitteeMemberRoles.Queries.Results;
using CommitteeCore.Entities;

namespace CommitteeApplication.Mappers.CommitteeMemberRoles
{
    public class CommiMembRolesMappingProfile : Profile
    {
        public CommiMembRolesMappingProfile()
        {
            CreateMap<CommitteeMemberRole, GetCommiMembRolesResponse>().ReverseMap();

            // Add item → Entity
            CreateMap<SingleCommiMembRoleItem, CommitteeMemberRole>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // Update command → Entity
            CreateMap<UpdateCommiMembRolesCommand, CommitteeMemberRole>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CommitteeMemberId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
