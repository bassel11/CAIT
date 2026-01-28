using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaItems.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AgendaItemVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.AgendaItems.Commands.Handlers
{
    public class AddAgendaItemAttachmentCommandHandler : ICommandHandler<AddAgendaItemAttachmentCommand, Result<Guid>>
    {
        private readonly IMeetingRepository _meetingRepository;

        public AddAgendaItemAttachmentCommandHandler(IMeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }

        public async Task<Result<Guid>> Handle(AddAgendaItemAttachmentCommand request, CancellationToken cancellationToken)
        {
            // 1. جلب الاجتماع مع الأجندة (لأن الأجندة هي من يملك المرفقات)
            // ملاحظة: نحتاج لـ Include للمرفقات إذا كنا سنتحقق من التكرار، لكن للإضافة فقط يكفي جلب الأجندة
            var meeting = await _meetingRepository.GetWithAgendaAsync(MeetingId.Of(request.MeetingId), cancellationToken);

            if (meeting == null) return Result<Guid>.Failure("Meeting not found.");

            var agendaItemId = AgendaItemId.Of(request.AgendaItemId);
            var agendaItem = meeting.AgendaItems.FirstOrDefault(x => x.Id == agendaItemId);

            if (agendaItem == null) return Result<Guid>.Failure("Agenda item not found.");

            try
            {
                // 2. إضافة المرفق
                agendaItem.AddAttachment(request.FileName, request.FileUrl, request.ContentType);

                // 3. الحفظ
                await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

                // استرجاع الـ ID للمرفق الجديد (آخر واحد مضاف)
                var newAttachment = agendaItem.Attachments.Last();
                return Result<Guid>.Success(newAttachment.Id.Value, "Attachment added successfully.");
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Failed to add attachment: {ex.Message}");
            }
        }
    }
}
