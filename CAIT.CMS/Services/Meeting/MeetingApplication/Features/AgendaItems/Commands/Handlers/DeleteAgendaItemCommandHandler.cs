using MediatR;
using MeetingApplication.Exceptions;
using MeetingApplication.Features.AgendaItems.Commands.Models;
using MeetingCore.Entities;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.AgendaItems.Commands.Handlers
{
    public class DeleteAgendaItemCommandHandler : IRequestHandler<DeleteAgendaItemCommand, Unit>
    {
        #region Fields

        private readonly IAgendaRepository _agendaRepository;
        #endregion

        #region Constructor
        public DeleteAgendaItemCommandHandler(IAgendaRepository agendaRepository)
        {
            _agendaRepository = agendaRepository;
        }

        #endregion

        #region Actions

        public async Task<Unit> Handle(DeleteAgendaItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _agendaRepository.GetByIdAsync(request.Id);

            if (item == null)
                throw new AgendaNotFoundException(nameof(AgendaItem), request.Id);

            await _agendaRepository.DeleteAsync(item);

            return Unit.Value;
        }

        #endregion
    }
}
