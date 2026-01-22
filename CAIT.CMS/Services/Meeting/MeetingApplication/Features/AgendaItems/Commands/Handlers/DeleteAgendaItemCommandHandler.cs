using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.AgendaItems.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AgendaItemVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.AgendaItems.Commands.Handlers
{
    public class DeleteAgendaItemCommandHandler
        : ICommandHandler<DeleteAgendaItemCommand, Result>
    {
        private readonly IMeetingRepository _meetingRepository;

        public DeleteAgendaItemCommandHandler(IMeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }

        public async Task<Result> Handle(DeleteAgendaItemCommand request, CancellationToken cancellationToken)
        {
            var meeting = await _meetingRepository.GetWithAgendaAsync(MeetingId.Of(request.MeetingId), cancellationToken);
            if (meeting == null) return Result.Failure("Meeting not found.");

            try
            {
                meeting.RemoveAgendaItem(AgendaItemId.Of(request.AgendaItemId));
                await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success("Agenda Item removed successfully.");
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
