using BuildingBlocks.Shared.Exceptions;
using MeetingCore.DomainServices;
using MeetingCore.Enums.MeetingEnums;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingInfrastructure.DomainServices
{
    public class MeetingOverlapDomainService : IMeetingOverlapDomainService
    {
        private readonly IMeetingRepository _meetingRepository;

        public MeetingOverlapDomainService(IMeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }

        public async Task ValidateRoomAvailabilityAsync(
            DateTime startDate,
            DateTime endDate,
            MeetingLocation location,
            MeetingId? currentMeetingId = null,
            CancellationToken cancellationToken = default)
        {
            // 1. إذا لم يكن الاجتماع في قاعة (Online)، لا يوجد تعارض قاعات
            if (location.Type == LocationType.Online || string.IsNullOrWhiteSpace(location.RoomName))
            {
                return;
            }

            // 2. التحقق عبر الـ Repository
            bool hasConflict = await _meetingRepository.HasConflictAsync(
                startDate,
                endDate,
                location.RoomName,
                currentMeetingId,
                cancellationToken);

            // 3. رمي الاستثناء في حال وجود تعارض
            if (hasConflict)
            {
                throw new DomainException($"The room '{location.RoomName}' is already booked during the selected time ({startDate} - {endDate}).");
            }
        }
    }
}
