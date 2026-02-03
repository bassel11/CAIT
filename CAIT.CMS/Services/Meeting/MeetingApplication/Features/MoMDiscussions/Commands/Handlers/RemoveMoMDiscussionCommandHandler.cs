using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMDiscussions.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;
using MeetingCore.ValueObjects.MoMDiscussionVO;

namespace MeetingApplication.Features.MoMDiscussions.Commands.Handlers
{
    public class RemoveMoMDiscussionCommandHandler : ICommandHandler<RemoveMoMDiscussionCommand, Result>
    {
        private readonly IMinutesRepository _repo;
        private readonly ICurrentUserService _user;

        public RemoveMoMDiscussionCommandHandler(IMinutesRepository repo, ICurrentUserService user)
        {
            _repo = repo;
            _user = user;
        }

        public async Task<Result> Handle(RemoveMoMDiscussionCommand req, CancellationToken ct)
        {
            var mom = await _repo.GetFullGraphByMeetingIdAsync(MeetingId.Of(req.MeetingId), ct);

            if (mom == null) return Result.Failure("Minutes not found.");

            try
            {
                // 2. استدعاء دالة الدومين للحذف
                mom.RemoveDiscussion(MoMDiscussionId.Of(req.TopicId));

                // 3. الحفظ
                await _repo.UnitOfWork.SaveChangesAsync(ct);

                return Result.Success("Discussion topic removed successfully.");
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
