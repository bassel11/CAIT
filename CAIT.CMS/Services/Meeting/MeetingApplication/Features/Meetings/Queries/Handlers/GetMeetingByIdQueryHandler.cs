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
    public class GetMeetingByIdQueryHandler : ResponseHandler, IRequestHandler<GetMeetingByIdQuery, Response<GetMeetingResponse>>
    {
        #region Fields

        private readonly IMeetingRepository _meetingRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;

        #endregion

        #region Constructor

        public GetMeetingByIdQueryHandler(IMeetingRepository meetingRepository
                                        , IMapper mapper
                                        , IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            _meetingRepository = meetingRepository;
            _mapper = mapper;
        }

        #endregion

        #region Actions
        public async Task<Response<GetMeetingResponse>> Handle(GetMeetingByIdQuery request, CancellationToken cancellationToken)
        {
            var meetingList = await _meetingRepository.GetByIdAsync(request.Id);

            if (meetingList == null)
                return NotFound<GetMeetingResponse>(
                    _stringLocalizer[SharedResourcesKeys.NotFound]);

            var result = _mapper.Map<GetMeetingResponse>(meetingList);

            return Success(result);
        }

        #endregion
    }
}
