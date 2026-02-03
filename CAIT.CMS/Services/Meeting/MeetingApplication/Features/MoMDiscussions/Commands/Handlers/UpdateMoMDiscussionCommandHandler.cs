using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMDiscussions.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;
using MeetingCore.ValueObjects.MoMDiscussionVO;

namespace MeetingApplication.Features.MoMDiscussions.Commands.Handlers
{
    public class UpdateMoMDiscussionCommandHandler : ICommandHandler<UpdateMoMDiscussionCommand, Result>
    {
        private readonly IMinutesRepository _repo;
        private readonly ICurrentUserService _user;

        public UpdateMoMDiscussionCommandHandler(IMinutesRepository repo, ICurrentUserService user)
        {
            _repo = repo;
            _user = user;
        }

        public async Task<Result> Handle(UpdateMoMDiscussionCommand req, CancellationToken ct)
        {
            var mom = await _repo.GetFullGraphByMeetingIdAsync(MeetingId.Of(req.MeetingId), ct);
            if (mom == null) return Result.Failure("Minutes not found.");

            try
            {
                // استدعاء الدالة في الـ Aggregate
                mom.UpdateTopicDiscussion(MoMDiscussionId.Of(req.TopicId), req.Content);
                await _repo.UnitOfWork.SaveChangesAsync(ct);
                return Result.Success("Topic discussion updated.");
            }
            catch (DomainException ex) { return Result.Failure(ex.Message); }
        }
    }
}
