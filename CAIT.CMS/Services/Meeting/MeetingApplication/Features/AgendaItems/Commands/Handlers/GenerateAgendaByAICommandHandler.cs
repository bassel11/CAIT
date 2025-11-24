using AutoMapper;
using MediatR;
using MeetingApplication.Exceptions;
using MeetingApplication.Features.AgendaItems.Commands.Models;
using MeetingApplication.Features.AgendaItems.Queries.Results;
using MeetingCore.Entities;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.AgendaItems.Commands.Handlers
{
    public class GenerateAgendaByAICommandHandler : IRequestHandler<GenerateAgendaByAICommand, List<GetAgendaItemResponse>>
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly IMapper _mapper;

        public GenerateAgendaByAICommandHandler(IMeetingRepository meetingRepository, IMapper mapper)
        {
            _meetingRepository = meetingRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAgendaItemResponse>> Handle(GenerateAgendaByAICommand req, CancellationToken ct)
        {
            var meeting = await _meetingRepository.GetByIdAsync(req.MeetingId)
                ?? throw new MeetingNotFoundException(nameof(Meeting), req.MeetingId);

            // Simulated AI logic (replace with your model)
            var aiItems = new List<GetAgendaItemResponse>
        {
            new() { Id = Guid.NewGuid(), MeetingId = req.MeetingId, Title = "Opening & Intro", SortOrder = 1 },
            new() { Id = Guid.NewGuid(), MeetingId = req.MeetingId, Title = "Review Previous Decisions", SortOrder = 2 },
            new() { Id = Guid.NewGuid(), MeetingId = req.MeetingId, Title = "Discussion: " + req.Purpose, SortOrder = 3 },
            new() { Id = Guid.NewGuid(), MeetingId = req.MeetingId, Title = "Action Points", SortOrder = 4 }
        };

            return aiItems;
        }
    }
}
