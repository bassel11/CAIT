using AutoMapper;
using MediatR;
using MeetingApplication.Features.Meetings.Queries.Models;
using MeetingApplication.Features.Meetings.Queries.Results;
using MeetingApplication.Resources;
using MeetingApplication.Responses;
using MeetingCore.Repositories;
using Microsoft.Extensions.Localization;

namespace MeetingApplication.Features.Meetings.Queries.Handlers
{
    public class GetMeetingsByCommitteeIdQueryHandler
        : ResponseHandler,
          IRequestHandler<GetMeetingsByCommitteeIdQuery, Response<IEnumerable<GetMeetingResponse>>>
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _stringlocalizer;

        public GetMeetingsByCommitteeIdQueryHandler(
              IMeetingRepository meetingRepository,
              IMapper mapper,
              IStringLocalizer<SharedResources> stringlocalizer)
            : base(stringlocalizer)
        {
            _meetingRepository = meetingRepository;
            _mapper = mapper;
            _stringlocalizer = stringlocalizer;
        }

        public async Task<Response<IEnumerable<GetMeetingResponse>>> Handle(
             GetMeetingsByCommitteeIdQuery request,
             CancellationToken cancellationToken)
        {
            var meetings = await _meetingRepository.GetByCommitteeIdAsync(
                request.CommitteeId,
                cancellationToken
            );

            if (!meetings.Any())
                return NotFound<IEnumerable<GetMeetingResponse>>(_stringlocalizer[SharedResourcesKeys.NotFound]);

            var result = _mapper.Map<IEnumerable<GetMeetingResponse>>(meetings);

            return Success(result);
        }

    }
}
