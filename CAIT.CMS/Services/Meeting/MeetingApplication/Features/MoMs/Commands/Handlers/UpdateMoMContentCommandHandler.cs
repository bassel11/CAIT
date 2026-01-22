using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class UpdateMoMCommandContentHandler : ICommandHandler<UpdateMoMContentCommand, Result>
    {
        private readonly IMinutesRepository _repo;
        private readonly ICurrentUserService _user;

        public UpdateMoMCommandContentHandler(IMinutesRepository repo, ICurrentUserService user)
        {
            _repo = repo;
            _user = user;
        }

        public async Task<Result> Handle(UpdateMoMContentCommand req, CancellationToken ct)
        {
            var mom = await _repo.GetWithVersionsByMeetingIdAsync(MeetingId.Of(req.MeetingId), ct);
            if (mom == null) return Result.Failure("Minutes not found.");

            try
            {
                mom.UpdateContent(req.Content, UserId.Of(_user.UserId));
                await _repo.UnitOfWork.SaveChangesAsync(ct);
                return Result.Success("Minutes content updated.");
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
