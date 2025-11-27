using AutoMapper;
using MediatR;
using MeetingApplication.Features.MoMs.Queries.Models;
using MeetingApplication.Features.MoMs.Queries.Results;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.MoMs.Queries.Handlers
{
    public class GetMoMsForMeetingQueryHandler
        : IRequestHandler<GetMoMsForMeetingQuery, List<GetMinutesResponse>>
    {
        private readonly IMoMRepository _repo;
        private readonly IMapper _mapper;

        public GetMoMsForMeetingQueryHandler(IMoMRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<GetMinutesResponse>> Handle(GetMoMsForMeetingQuery req, CancellationToken ct)
        {
            var list = await _repo.GetByMeetingsIdAsync(req.MeetingId, ct);

            return _mapper.Map<List<GetMinutesResponse>>(list);

        }
    }
}
