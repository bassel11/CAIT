using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMAttachments.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMAttachments.Commands.Handlers
{
    public class AddMoMAttachmentCommandHandler
        : ICommandHandler<AddMoMAttachmentCommand, Result<Guid>>
    {
        private readonly IMinutesRepository _repo;
        private readonly ICurrentUserService _user;

        public AddMoMAttachmentCommandHandler(IMinutesRepository repo, ICurrentUserService user)
        {
            _repo = repo;
            _user = user;
        }

        public async Task<Result<Guid>> Handle(AddMoMAttachmentCommand req, CancellationToken ct)
        {
            // ✅ تحميل المرفقات فقط للتحقق من القيود (مثلاً عدم تكرار الاسم)
            var mom = await _repo.GetWithAttachmentsByMeetingIdAsync(MeetingId.Of(req.MeetingId), ct);
            if (mom == null) return Result<Guid>.Failure("Minutes not found.");

            try
            {
                // التحقق من تكرار الاسم (Business Rule)
                if (mom.Attachments.Any(a => a.FileName == req.FileName))
                    return Result<Guid>.Failure("A file with this name already exists.");

                mom.AddAttachment(
                    req.FileName,
                    req.ContentType,
                    req.SizeInBytes,
                    req.StoragePath,
                    UserId.Of(_user.UserId)
                );

                await _repo.UnitOfWork.SaveChangesAsync(ct);

                // نحتاج معرف المرفق الجديد لإرجاعه
                var newAttachment = mom.Attachments.Last();
                return Result<Guid>.Success(newAttachment.Id.Value, "Attachment added successfully.");
            }
            catch (DomainException ex) { return Result<Guid>.Failure(ex.Message); }
        }
    }
}
