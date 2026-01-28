using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaItems.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AgendaItemAttachmentVO;
using MeetingCore.ValueObjects.AgendaItemVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.AgendaItems.Commands.Handlers
{
    public class RemoveAgendaItemAttachmentCommandHandler : ICommandHandler<RemoveAgendaItemAttachmentCommand, Result>
    {
        private readonly IMeetingRepository _meetingRepository;

        public RemoveAgendaItemAttachmentCommandHandler(IMeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }

        public async Task<Result> Handle(RemoveAgendaItemAttachmentCommand request, CancellationToken cancellationToken)
        {
            // نحتاج هنا لتضمين المرفقات عند الجلب للتمكن من حذفها
            // يجب تحديث IMeetingRepository لإضافة Include(Attachments) في GetWithAgendaAsync أو دالة جديدة
            var meeting = await _meetingRepository.GetWithAgendaAsync(MeetingId.Of(request.MeetingId), cancellationToken);
            if (meeting == null) return Result.Failure("Meeting not found.");

            var agendaItem = meeting.AgendaItems.FirstOrDefault(x => x.Id == AgendaItemId.Of(request.AgendaItemId));
            if (agendaItem == null) return Result.Failure("Agenda item not found.");

            // الحذف
            agendaItem.RemoveAttachment(AgendaItemAttachmentId.Of(request.AttachmentId));

            await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success("Attachment removed successfully.");
        }
    }
}
