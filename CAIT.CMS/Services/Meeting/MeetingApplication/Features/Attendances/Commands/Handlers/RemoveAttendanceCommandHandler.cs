using MediatR;
using MeetingApplication.Exceptions;
using MeetingApplication.Features.Attendances.Commands.Models;
using MeetingCore.Entities;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.Attendances.Commands.Handlers
{
    public class RemoveAttendanceCommandHandler : IRequestHandler<RemoveAttendanceCommand, Unit>
    {
        #region Fields
        private readonly IAttendanceRepository _repo;
        #endregion

        #region Constructor
        public RemoveAttendanceCommandHandler(IAttendanceRepository repo) => _repo = repo;

        #endregion

        #region Actions
        public async Task<Unit> Handle(RemoveAttendanceCommand req, CancellationToken ct)
        {
            var rec = await _repo.GetByIdAsync(req.Id);

            if (rec == null)
            {
                throw new AttendanceNotFoundException(nameof(Attendance), req.Id);
            }

            await _repo.DeleteAsync(rec);
            await _repo.SaveChangesAsync(ct);
            return Unit.Value;
        }
        #endregion
    }
}
