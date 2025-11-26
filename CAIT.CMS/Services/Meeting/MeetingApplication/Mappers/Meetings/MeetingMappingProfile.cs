using AutoMapper;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingApplication.Features.Meetings.Commands.Results;
using MeetingApplication.Features.Meetings.Queries.Results;
using MeetingCore.Entities;

namespace MeetingApplication.Mappers.Meetings
{
    public class MeetingMappingProfile : Profile
    {
        public MeetingMappingProfile()
        {
            CreateMap<CreateMeetingCommand, Meeting>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id يولد تلقائياً;
            CreateMap<UpdateMeetingCommand, Meeting>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id يولد تلقائياً;
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
