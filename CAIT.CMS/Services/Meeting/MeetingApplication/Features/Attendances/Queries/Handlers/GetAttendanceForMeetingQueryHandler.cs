using AutoMapper;
using MediatR;
using MeetingApplication.Features.Attendances.Queries.Models;
using MeetingApplication.Features.Attendances.Queries.Results;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.Attendances.Queries.Handlers
{
    public class GetAttendanceForMeetingQueryHandler : IRequestHandler<GetAttendanceForMeetingQuery, List<GetAttendanceResponse>>
    {
        #region Fields
        private readonly IAttendanceRepository _repo;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public GetAttendanceForMeetingQueryHandler(IAttendanceRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        #endregion

        #region Actions
        public async Task<List<GetAttendanceResponse>> Handle(GetAttendanceForMeetingQuery req, CancellationToken ct)
        {
            var items = await _repo.GetByMeetingAsync(req.MeetingId, ct);

            return _mapper.Map<List<GetAttendanceResponse>>(items);
        }
        #endregion
    }
}
