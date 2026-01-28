using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingCore.DomainServices
{
    public interface IMeetingOverlapDomainService
    {
        /// <summary>
        /// يتحقق مما إذا كانت القاعة متاحة في الوقت المحدد.
        /// يرمي DomainException في حال وجود تعارض.
        /// </summary>
        Task ValidateRoomAvailabilityAsync(
            DateTime startDate,
            DateTime endDate,
            MeetingLocation location,
            MeetingId? currentMeetingId = null,
            CancellationToken cancellationToken = default);
    }
}
