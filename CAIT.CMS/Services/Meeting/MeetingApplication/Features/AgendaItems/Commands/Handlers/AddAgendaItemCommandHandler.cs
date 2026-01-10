using MediatR;
using MeetingApplication.Features.AgendaItems.Commands.Models;
using MeetingCore.Entities;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.AgendaItems.Commands.Handlers
{
    public class AddAgendaItemCommandHandler : IRequestHandler<AddAgendaItemCommand, Guid>
    {
        #region Fields
        private readonly IAgendaRepository _agendaRepository;
        private readonly IMeetingRepository _meetingRepository;
        private readonly ICurrentUserService _user;
        #endregion

        #region Constructor
        public AddAgendaItemCommandHandler(
        IAgendaRepository agendaRepository,
        IMeetingRepository meetingRepository,
        ICurrentUserService user)
        {
            _agendaRepository = agendaRepository;
            _meetingRepository = meetingRepository;
            _user = user;
        }
        #endregion

        #region Actions
        public async Task<Guid> Handle(AddAgendaItemCommand req, CancellationToken ct)
        {
            var meeting = await _meetingRepository.GetByIdAsync(req.MeetingId);


            if (meeting == null)
                throw new NotFoundException("Meeting", req.MeetingId);

            var item = new AgendaItem
            {
                Id = Guid.NewGuid(),
                MeetingId = req.MeetingId,
                Title = req.Title,
                Description = req.Description,
                SortOrder = req.SortOrder,
                CreatedAt = DateTime.UtcNow
            };

            await _agendaRepository.AddAsync(item);
            await _meetingRepository.SaveChangesAsync(ct);
            return item.Id;
        }
        #endregion

    }
}
