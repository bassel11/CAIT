using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingApplication.Interfaces;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class GenerateMoMByAICommandHandler : ICommandHandler<GenerateMoMByAICommand, Result<string>>
    {
        private readonly IMeetingRepository _meetingRepo;
        private readonly IMinutesAIService _aiService;

        public GenerateMoMByAICommandHandler(IMeetingRepository meetingRepo, IMinutesAIService aiService)
        {
            _meetingRepo = meetingRepo;
            _aiService = aiService;
        }

        public async Task<Result<string>> Handle(GenerateMoMByAICommand req, CancellationToken ct)
        {
            var meeting = await _meetingRepo.GetByIdAsync(MeetingId.Of(req.MeetingId), ct);
            if (meeting == null) return Result<string>.Failure("Meeting not found.");

            var promptContext = $"Meeting Title: {meeting.Title.Value}";
            var content = await _aiService.GenerateContentAsync(promptContext, req.Transcript, ct);

            return Result<string>.Success(content);
        }
    }
}
