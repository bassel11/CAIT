using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Attendances.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Attendances.Commands.Handlers
{
    public class BulkCheckInCommandHandler : ICommandHandler<BulkCheckInCommand, Result>
    {
        private readonly IMeetingRepository _meetingRepository;

        public BulkCheckInCommandHandler(IMeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }

        public async Task<Result> Handle(BulkCheckInCommand request, CancellationToken cancellationToken)
        {
            // 1. Load Aggregate
            // نحتاج Attendees للتحقق وتحديث حالتهم
            var meeting = await _meetingRepository.GetWithAttendeesAsync(MeetingId.Of(request.MeetingId), cancellationToken);

            if (meeting == null)
                return Result.Failure("Meeting not found.");

            try
            {
                // 2. Map Command DTOs to Domain Arguments
                // نحول القائمة إلى Tuple يحتوي على (UserId, Status, IsProxy, ProxyName)
                // ليتمكن الدومين من معالجتها
                var domainEntries = request.Items
                    .Select(x => (
                        UserId: UserId.Of(x.UserId),
                        Status: x.Status,
                        IsProxy: x.IsProxy,        // ✅ تمرير
                        ProxyName: x.ProxyName     // ✅ تمرير
                    ))
                    .ToList();

                // 3. Execute Domain Logic
                meeting.BulkCheckIn(domainEntries);

                // 4. Save
                await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

                // نتحقق من النصاب بعد التعديل لإرجاع رسالة ذكية
                var quorumMsg = meeting.IsQuorumMet() ? "Quorum Met ✅" : "Waiting for Quorum ⏳";
                return Result.Success($"Bulk check-in completed. {quorumMsg}");
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
