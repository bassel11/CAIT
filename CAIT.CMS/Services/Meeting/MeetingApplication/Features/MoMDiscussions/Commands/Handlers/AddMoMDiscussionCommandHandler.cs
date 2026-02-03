using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMDiscussions.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMDiscussions.Commands.Handlers
{
    public class AddMoMDiscussionCommandHandler : ICommandHandler<AddMoMDiscussionCommand, Result<Guid>>
    {
        private readonly IMinutesRepository _repo;
        private readonly ICurrentUserService _user;

        public AddMoMDiscussionCommandHandler(IMinutesRepository repo, ICurrentUserService user)
        {
            _repo = repo;
            _user = user;
        }

        public async Task<Result<Guid>> Handle(AddMoMDiscussionCommand req, CancellationToken ct)
        {
            // 1. جلب المحضر (نحتاج النقاشات لذلك نستخدم FullGraph أو نضمن تحميلها)
            // في حالة الإضافة، يكفي جلب المحضر الأساسي لأننا سنضيف عليه، لكن للأمان نستخدم FullGraph
            var mom = await _repo.GetFullGraphByMeetingIdAsync(MeetingId.Of(req.MeetingId), ct);

            if (mom == null) return Result<Guid>.Failure("Minutes not found.");

            try
            {
                // 2. استدعاء دالة الدومين التي أضفناها سابقاً
                // هذه الدالة ستقوم بإنشاء MoMDiscussion جديد ووضعه في القائمة
                // وتلقائياً ستضع AgendaItemId = null لأنه Ad-hoc
                mom.AddAdHocDiscussion(req.Title, req.Content);

                // 3. الحفظ
                await _repo.UnitOfWork.SaveChangesAsync(ct);

                // 4. استرجاع Id العنصر المضاف حديثاً (سيكون الأخير في القائمة)
                var addedDiscussion = mom.Discussions.Last();

                return Result<Guid>.Success(addedDiscussion.Id.Value, "Ad-hoc discussion added successfully.");
            }
            catch (DomainException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }
    }
}
