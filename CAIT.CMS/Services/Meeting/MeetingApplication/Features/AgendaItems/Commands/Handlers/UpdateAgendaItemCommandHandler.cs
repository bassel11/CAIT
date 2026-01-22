using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaItems.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AgendaItemVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.AgendaItems.Commands.Handlers
{
    public class UpdateAgendaItemCommandHandler : ICommandHandler<UpdateAgendaItemCommand, Result>
    {
        private readonly IMeetingRepository _meetingRepository;

        public UpdateAgendaItemCommandHandler(IMeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }

        public async Task<Result> Handle(UpdateAgendaItemCommand request, CancellationToken cancellationToken)
        {
            var meeting = await _meetingRepository.GetWithAgendaAsync(MeetingId.Of(request.MeetingId), cancellationToken);
            if (meeting == null) return Result.Failure("Meeting not found.");

            try
            {
                meeting.UpdateAgendaItem(
                    AgendaItemId.Of(request.AgendaItemId),
                    AgendaItemTitle.Of(request.Title),
                    request.Description,
                    SortOrder.Of(request.SortOrder),
                    request.DurationMinutes.HasValue ? Duration.FromMinutes(request.DurationMinutes.Value) : null,
                    request.PresenterId.HasValue ? PresenterId.Of(request.PresenterId.Value) : null
                );

                await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success("Agenda Item updated successfully.");
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
