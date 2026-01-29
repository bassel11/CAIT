using MeetingCore.Enums.AttendanceEnums;

namespace MeetingCore.Events.AttendanceEvents
{
    // 1. عند تأكيد الحضور (RSVP)
    public record MeetingAttendeeRSVPedEvent(
        Guid MeetingId,
        Guid MemberId,
        RSVPStatus RSVP,
        DateTime Timestamp
    ) : IDomainEvent;

    // 2. عند تسجيل الحضور (Check-In)
    public record MeetingAttendeeCheckedInEvent(
        Guid MeetingId,
        Guid MemberId,
        AttendanceStatus Status,
        bool IsRemote,
        DateTime Timestamp,
        bool IsQuorumMet
    ) : IDomainEvent;

    // 3. عند إضافة عضو جديد
    public record MeetingAttendeeAddedEvent(
        Guid MeetingId,
        Guid MemberId,
        AttendanceRole Role
    ) : IDomainEvent;

    // 4. عند إزالة عضو
    public record MeetingAttendeeRemovedEvent(
        Guid MeetingId,
        Guid MemberId
    ) : IDomainEvent;

    public record BulkCheckInItem(Guid MemberId, AttendanceStatus Status);

    public record MeetingAttendeesBulkCheckedInEvent(
        Guid MeetingId,
        List<BulkCheckInItem> CheckedInItems,
        DateTime Timestamp,
        bool IsQuorumMet // ✅ هام جداً: حالة النصاب الجديدة بعد التحديث الجماعي
    ) : IDomainEvent;
}
