using MediatR;
using MeetingApplication.Common.DateTimeProvider;
using MeetingApplication.Exceptions;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class ArchiveMoMCommandHandler : IRequestHandler<ArchiveMoMCommand, Unit>
    {
        private readonly IMoMRepository _momRepo;
        private readonly IDateTimeProvider _clock;
        public ArchiveMoMCommandHandler(IMoMRepository momRepo, IDateTimeProvider clock) { _momRepo = momRepo; _clock = clock; }
        public async Task<Unit> Handle(ArchiveMoMCommand req, CancellationToken ct)
        {
            var mom = await _momRepo.GetByIdAsync(req.MoMId);
            if (mom == null)
            {
                throw new MoMNotFoundException(nameof(MinutesOfMeeting), req.MoMId);
            }
            mom.Status = MoMStatus.Archived;
            //mom.UpdatedAt = _clock.UtcNow;
            await _momRepo.UpdateAsync(mom);
            await _momRepo.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
