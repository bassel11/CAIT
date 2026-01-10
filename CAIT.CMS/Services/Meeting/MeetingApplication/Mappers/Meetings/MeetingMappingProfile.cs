using AutoMapper;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingApplication.Features.Meetings.Commands.Results;
using MeetingApplication.Features.Meetings.Queries.Results;
using MeetingCore.Entities;
using MeetingCore.Enums; // تأكد من إضافة هذا الـ Namespace

namespace MeetingApplication.Mappers.Meetings
{
    public class MeetingMappingProfile : Profile
    {
        public MeetingMappingProfile()
        {
            // ---------------------------------------------------------
            // 1. التحويل من Command إلى Entity (هنا يحدث الخطأ 500)
            // ---------------------------------------------------------
            CreateMap<CreateMeetingCommand, Meeting>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id يولد تلقائياً

                // ✅ الحل الجذري: تحويل آمن للـ Enum
                .ForMember(dest => dest.RecurrenceType, opt => opt.MapFrom(src =>
                    ParseRecurrenceType(src.RecurrenceType)));

            // نفس الشيء للتحديث
            CreateMap<UpdateMeetingCommand, Meeting>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RecurrenceType, opt => opt.MapFrom(src =>
                    ParseRecurrenceType(src.RecurrenceType ?? ""))); // Handling nulls for update if needed


            // ---------------------------------------------------------
            // 2. التحويل العكسي (Entity -> Response)
            // ---------------------------------------------------------
            CreateMap<Meeting, CreateMeetingResponse>().ReverseMap();
            CreateMap<Meeting, UpdateMeetingResponse>().ReverseMap();
            CreateMap<Meeting, GetMeetingResponse>().ReverseMap();
        }

        // دالة مساعدة صغيرة لجعل التحويل آمناً
        private static RecurrenceType ParseRecurrenceType(string recurrenceTypeString)
        {
            if (string.IsNullOrWhiteSpace(recurrenceTypeString))
                return RecurrenceType.None;

            // محاولة التحويل، إذا فشلت (مثل "string" أو "ttt") نرجع None
            if (Enum.TryParse<RecurrenceType>(recurrenceTypeString, true, out var result))
            {
                return result; // التحويل نجح (مثلاً Weekly)
            }

            return RecurrenceType.None; // التحويل فشل (قيمة خاطئة)، نرجع الافتراضي ولا نكسر النظام
        }
    }
}