using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class ArchiveMoMCommandHandler : ICommandHandler<ArchiveMoMCommand, Result>
    {
        private readonly IMinutesRepository _repo;
        private readonly ICurrentUserService _user;

        public ArchiveMoMCommandHandler(IMinutesRepository repo, ICurrentUserService user)
        {
            _repo = repo;
            _user = user;
        }

        public async Task<Result> Handle(ArchiveMoMCommand req, CancellationToken ct)
        {
            // استخدام الاستعلام الخفيف
            var mom = await _repo.GetByMeetingIdSimpleAsync(MeetingId.Of(req.MeetingId), ct);

            if (mom == null) return Result.Failure("Minutes not found.");

            try
            {
                // الدالة التي أضفناها للكيان
                mom.Archive();
                await _repo.UnitOfWork.SaveChangesAsync(ct);
                return Result.Success("Minutes archived successfully.");
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
