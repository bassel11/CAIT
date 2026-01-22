using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMAttachments.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;
using MeetingCore.ValueObjects.MoMAttachmentVO;

namespace MeetingApplication.Features.MoMAttachments.Commands.Handlers
{
    public class RemoveMoMAttachmentCommandHandler :
        ICommandHandler<RemoveMoMAttachmentCommand, Result>
    {
        private readonly IMinutesRepository _repo;
        private readonly ICurrentUserService _user;

        public RemoveMoMAttachmentCommandHandler(IMinutesRepository repo, ICurrentUserService user)
        {
            _repo = repo;
            _user = user;
        }

        public async Task<Result> Handle(RemoveMoMAttachmentCommand req, CancellationToken ct)
        {
            // ✅ تحميل المرفقات للحذف
            var mom = await _repo.GetWithAttachmentsByMeetingIdAsync(MeetingId.Of(req.MeetingId), ct);
            if (mom == null) return Result.Failure("Minutes not found.");

            try
            {
                mom.RemoveAttachment(MoMAttachmentId.Of(req.AttachmentId));
                await _repo.UnitOfWork.SaveChangesAsync(ct);
                return Result.Success("Attachment removed successfully.");
            }
            catch (DomainException ex) { return Result.Failure(ex.Message); }
        }
    }
}
