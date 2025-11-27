using MediatR;
using MeetingApplication.Common.CurrentUser;
using MeetingApplication.Common.DateTimeProvider;
using MeetingApplication.Exceptions;
using MeetingApplication.Features.MoMAttachments.Commands.Models;
using MeetingApplication.Interfaces.Integrations;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.MoMAttachments.Commands.Handlers
{
    public class AddMoMAttachmentCommandHandler
        : IRequestHandler<AddMoMAttachmentCommand, Guid>
    {
        private readonly IMoMRepository _momRepo;
        private readonly IMoMAttachmentRepository _attchRepo;
        private readonly IStorageService _storage;
        private readonly IDateTimeProvider _clock;
        private readonly ICurrentUserService _user;

        public AddMoMAttachmentCommandHandler(IMoMRepository momRepo
                                            , IStorageService storage
                                            , IDateTimeProvider clock
                                            , ICurrentUserService user
                                            , IMoMAttachmentRepository attchRepo)
        {
            _momRepo = momRepo;
            _storage = storage;
            _clock = clock;
            _user = user;
            _attchRepo = attchRepo;
        }

        public async Task<Guid> Handle(AddMoMAttachmentCommand req, CancellationToken ct)
        {
            var mom = await _momRepo.GetByIdAsync(req.MoMId);
            if (mom == null)
            {
                throw new MoMNotFoundException(nameof(MinutesOfMeeting), req.MoMId);
            }

            if (mom.Status == MoMStatus.Approved || mom.Status == MoMStatus.Published)
                throw new DomainException("Cannot add attachments after approval/publication.");

            // save to storage
            var path = await _storage.SaveFileAsync(req.Content, req.FileName, req.ContentType, ct);

            // create attachment entity (we'll reuse MeetingIntegrationLog as attachments table isn't created previously; better to create MoMAttachments table)
            var att = new MoMAttachment
            {
                Id = Guid.NewGuid(),
                MoMId = mom.Id,
                FileName = req.FileName,
                StoragePath = path,
                ContentType = req.ContentType,
                UploadedAt = _clock.UtcNow,
                UploadedBy = _user.UserId == Guid.Empty ? Guid.Empty : _user.UserId
            };

            await _attchRepo.AddAsync(att);
            await _attchRepo.SaveChangesAsync(ct);

            return att.Id;
        }
    }
}
