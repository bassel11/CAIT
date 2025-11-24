using AutoMapper;
using MediatR;
using MeetingApplication.Features.AgendaItems.Queries.Models;
using MeetingApplication.Features.AgendaItems.Queries.Results;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.AgendaItems.Queries.Handlers
{
    public class GetAgendaItemsQueryHandler : IRequestHandler<GetAgendaItemsQuery, List<GetAgendaItemResponse>>
    {
        #region Fields
        private readonly IAgendaRepository _repo;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public GetAgendaItemsQueryHandler(IAgendaRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        #endregion

        #region Actions
        public async Task<List<GetAgendaItemResponse>> Handle(GetAgendaItemsQuery req, CancellationToken ct)
        {
            var items = await _repo.GetAgendaItemsByMeetingIdAsync(req.MeetingId, ct);
            return _mapper.Map<List<GetAgendaItemResponse>>(items);

        }
        #endregion
    }
}
