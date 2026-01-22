using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaItems.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AgendaItemVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.AgendaItems.Commands.Handlers
{
    public class AddAgendaItemCommandHandler : ICommandHandler<AddAgendaItemCommand, Result<Guid>>
    {
        private readonly IMeetingRepository _meetingRepository;

        public AddAgendaItemCommandHandler(IMeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }

        public async Task<Result<Guid>> Handle(AddAgendaItemCommand request, CancellationToken cancellationToken)
        {
            // 1. Load Aggregate (Meeting) with AgendaItems
            // نحتاج دالة في المستودع تجلب الاجتماع مع بنود الأعمال للتحقق من الترتيب
            var meeting = await _meetingRepository.GetWithAgendaAsync(MeetingId.Of(request.MeetingId), cancellationToken);

            if (meeting == null)
                return Result<Guid>.Failure("Meeting not found.");

            try
            {
                // 2. Prepare Value Objects
                var title = AgendaItemTitle.Of(request.Title);
                var order = SortOrder.Of(request.SortOrder);
                Duration? duration = request.DurationMinutes.HasValue
                    ? Duration.FromMinutes(request.DurationMinutes.Value)
                    : null;

                PresenterId? presenterId = request.PresenterId.HasValue
                    ? PresenterId.Of(request.PresenterId.Value)
                    : null;

                // 3. Execute Domain Behavior
                // لاحظ: لا يوجد إرجاع للـ ID من الدالة void، لذا سنبحث عنه أو نعدل الدالة لترجع الكيان المضاف
                // للتبسيط هنا، سنفترض أننا سنحصل على آخر عنصر مضاف (أو نعدل الدومين ليرجع الـ ID)
                meeting.AddAgendaItem(
                    title: title,
                    allocatedTime: duration,
                    sortOrder: order,
                    presenterId: presenterId,
                    description: request.Description
                );

                // 4. Save
                await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

                // استرجاع الـ ID للعنصر المضاف حديثاً (logic simple for demonstration)
                var addedItem = meeting.AgendaItems.Last();
                return Result<Guid>.Success(addedItem.Id.Value, "Agenda Item added successfully.");
            }
            catch (DomainException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }
    }
}
