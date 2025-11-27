using AutoMapper;
using MeetingApplication.Features.MoMs.Queries.Results;
using MeetingCore.Entities;

namespace MeetingApplication.Mappers.MoMs
{
    public class MoMProfile : Profile
    {
        public MoMProfile()
        {
            CreateMap<MinutesOfMeeting, GetMinutesResponse>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
        }
    }
}
