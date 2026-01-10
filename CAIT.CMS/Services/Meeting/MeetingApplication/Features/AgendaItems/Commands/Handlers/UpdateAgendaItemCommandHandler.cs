using BuildingBlocks.Shared.Exceptions;
using MediatR;
using MeetingApplication.Features.AgendaItems.Commands.Models;
using MeetingCore.Entities;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.AgendaItems.Commands.Handlers
{
    public class UpdateAgendaItemCommandHandler : IRequestHandler<UpdateAgendaItemCommand, Guid>
    {

        #region Fields
        private readonly IAgendaRepository _agendaRepository;
        #endregion

        #region Constructor
        public UpdateAgendaItemCommandHandler(IAgendaRepository agendaRepository)
        {
            _agendaRepository = agendaRepository;
        }

        #endregion

        #region  Actions
        public async Task<Guid> Handle(UpdateAgendaItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _agendaRepository.GetByIdAsync(request.Id);

            if (item == null)
                throw new NotFoundException(nameof(AgendaItem), request.Id);

            item.Title = request.Title;
            item.Description = request.Description;
            item.SortOrder = request.SortOrder;

            await _agendaRepository.UpdateAsync(item);

            return item.Id; // إرجاع الـ Guid
        }


        #endregion

    }
}
