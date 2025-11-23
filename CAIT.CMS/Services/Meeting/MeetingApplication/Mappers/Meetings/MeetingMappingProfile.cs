using AutoMapper;
using MeetingApplication.Features.Meetings.Commands.Results;
using MeetingApplication.Features.Meetings.Queries.Results;
using MeetingCore.Entities;

namespace MeetingApplication.Mappers.Meetings
{
    public class MeetingMappingProfile : Profile
    {
        public MeetingMappingProfile()
        {
            CreateMap<Meeting, CreateMeetingResponse>().ReverseMap();
            CreateMap<Meeting, UpdateMeetingResponse>().ReverseMap();
            CreateMap<Meeting, GetMeetingResponse>().ReverseMap();
            //CreateMap<AgendaItem, AgendaItemReponse>().ReverseMap();
            //CreateMap<Attendance, AttendanceRecordReponse>().ReverseMap();
            //CreateMap<MinutesOfMeeting, MinutesOfMeetingReponse>().ReverseMap();
            //CreateMap<MeetingDecision, DecisionReponse>().ReverseMap();
            //CreateMap<MeetingVote, VoteReponse>().ReverseMap();
        }
    }
}
