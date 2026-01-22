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
            var meeting = await _meetingRepository.GetWithAttendeesAsync(MeetingId.Of(request.MeetingId), cancellationToken);

            if (meeting == null)
                return Result.Failure("Meeting not found.");

            try
            {
                // 2. Map Command DTOs to Domain Types (Value Objects & Enums)
                // تحويل القائمة القادمة من الخارج إلى الشكل الذي يفهمه الدومين (Tuples)
                var domainEntries = request.Items
                    .Select(x => (UserId.Of(x.UserId), x.Status)) // ✅ لا حاجة للـ Casting هنا
                    .ToList();

                // 3. Execute Domain Logic
                meeting.BulkCheckIn(domainEntries);

                // 4. Save
                await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success("Bulk check-in completed successfully.");
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
