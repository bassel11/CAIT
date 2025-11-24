using AutoMapper;
using MeetingApplication.Features.Attendances.Queries.Results;
using MeetingCore.Entities;

namespace MeetingApplication.Mappers.Attendances
{
    public class AttendanceMappingProfile : Profile
    {
        public AttendanceMappingProfile()
        {
            CreateMap<Attendance, GetAttendanceResponse>();
        }
    }
}
