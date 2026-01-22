using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingCore.Entities;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class CreateMoMCommandHandler : ICommandHandler<CreateMoMCommand, Result<Guid>>
    {
        private readonly IMinutesRepository _momRepo;
        private readonly IMeetingRepository _meetingRepo;
        private readonly ICurrentUserService _user;

        public CreateMoMCommandHandler(
            IMinutesRepository momRepo,
            IMeetingRepository meetingRepo,
            ICurrentUserService user)
        {
            _momRepo = momRepo;
            _meetingRepo = meetingRepo;
            _user = user;
        }

        public async Task<Result<Guid>> Handle(CreateMoMCommand req, CancellationToken ct)
        {
            var meetingId = MeetingId.Of(req.MeetingId);

            // 1. التحقق من وجود الاجتماع
            var meeting = await _meetingRepo.GetByIdAsync(meetingId, ct);
            if (meeting == null)
                return Result<Guid>.Failure("Meeting not found.");

            // 2. التحقق من عدم وجود محضر سابق
            if (await _momRepo.ExistsForMeetingAsync(meetingId, ct))
                return Result<Guid>.Failure("Minutes already exist for this meeting.");

            // 3. الإنشاء
            var mom = new MinutesOfMeeting(
                meetingId,
                req.InitialContent,
                _user.UserId.ToString()
            );

            // 4. الحفظ
            await _momRepo.AddAsync(mom, ct);
            await _momRepo.UnitOfWork.SaveChangesAsync(ct);

            return Result<Guid>.Success(mom.Id.Value, "Minutes draft created successfully.");
        }
    }
}
