using AutoMapper;
using MediatR;
using MeetingApplication.Features.MoMs.Queries.Models;
using MeetingApplication.Features.MoMs.Queries.Results;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.MoMs.Queries.Handlers
{
    public class GetMoMByIdQueryHandler : IRequestHandler<GetMoMByIdQuery, GetMinutesResponse?>
    {
        private readonly IMoMRepository _repo;
        private readonly IMapper _mapper;

        public GetMoMByIdQueryHandler(IMoMRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<GetMinutesResponse?> Handle(GetMoMByIdQuery req, CancellationToken ct)
        {
            var mom = await _repo.GetByIdAsync(req.MoMId);
            if (mom == null) return null;
            return _mapper.Map<GetMinutesResponse>(mom);
        }
    }
}
