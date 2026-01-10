using BuildingBlocks.Shared.Exceptions;
using MediatR;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingApplication.Integrations;
using MeetingCore.Entities;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class GenerateMoMByAICommandHandler
        : IRequestHandler<GenerateMoMByAICommand, Guid>
    {
        private readonly ITeamsService _teams;
        private readonly IAIService _ai;
        private readonly IMoMRepository _momRepo;
        private readonly IMeetingRepository _meetRepo;
        private readonly IDateTimeProvider _clock;
        private readonly ICurrentUserService _user;
        private readonly IStorageService _storage;
        private readonly IEventBus _bus;

        public GenerateMoMByAICommandHandler(ITeamsService teams, IAIService ai, IMoMRepository momRepo, IMeetingRepository meetRepo, IDateTimeProvider clock, ICurrentUserService user, IStorageService storage, IEventBus bus)
        {
            _teams = teams;
            _ai = ai;
            _momRepo = momRepo;
            _meetRepo = meetRepo;
            _clock = clock;
            _user = user;
            _storage = storage;
            _bus = bus;
        }

        public async Task<Guid> Handle(GenerateMoMByAICommand req, CancellationToken ct)
        {
            var meeting = await _meetRepo.GetByIdAsync(req.MeetingId);
            if (meeting == null)
                throw new NotFoundException(nameof(Meeting), req.MeetingId);

            // get transcript (or notes)
            string transcript;
            if (req.FromTranscript)
            {
                transcript = ""; //await _teams.GetTranscriptAsync(req.MeetingId, ct) ?? throw new DomainException("Transcript not available");
            }
            else
            {
                transcript = ""; // If not transcript, AI may use uploaded notes - not implemented here
            }

            // call AI to generate MoM text
            var generated = await _ai.GenerateMoMDraftFromTranscriptAsync(transcript, req.MeetingId, ct);

            // Save generated MoM as draft
            var cmd = new CreateMoMCommand(req.MeetingId, generated);
            // create by reusing Create handler (prefer using mediator to avoid code duplication)
            // Assuming we have IMediator in DI:
            throw new NotImplementedException("Mediator dispatch to CreateMoMDraftCommand should be used here - wire IMediator in constructor and send command, then return id.");
        }
    }
}
