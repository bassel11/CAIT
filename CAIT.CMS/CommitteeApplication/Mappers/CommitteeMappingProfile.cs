using AutoMapper;
using CommitteeApplication.Commands;
using CommitteeApplication.Responses;
using CommitteeCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitteeApplication.Mappers
{
    public class CommitteeMappingProfile : Profile
    {
        public CommitteeMappingProfile()
        {
            CreateMap<Committee, CommitteeResponse>().ReverseMap();
            CreateMap<Committee, AddCommitteeCommand>().ReverseMap();
            CreateMap<Committee, UpdateCommitteeCommand>().ReverseMap();
        }
    }
}
