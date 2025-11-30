using MediatR;
using MeetingApplication.Common.CurrentUser;
using MeetingApplication.Common.DateTimeProvider;
using MeetingApplication.Exceptions;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingApplication.Interfaces.Integrations;
using MeetingApplication.Repositories;
using MeetingCore.Entities;
using MeetingCore.Repositories;
using System.Text.Json;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class ApproveMoMCommandHandler : IRequestHandler<ApproveMoMCommand, Unit>
    {
        private readonly IMoMRepository _momRepo;
        private readonly ICurrentUserService _user;
        private readonly IDateTimeProvider _clock;
        private readonly IStorageService _storage;
        private readonly IMoMAttachmentRepository _momatachmentRepository;
        private readonly IUnitOfWork _uow;
        private readonly IOutlookService _outlook;
        private readonly IEventBus _eventBus;
        private readonly IMeetingNotificationRepository _meetNotification;
        private readonly IPdfGenerator _pdf;
        private readonly IOutboxService _outbox;


        public ApproveMoMCommandHandler(IMoMRepository monRepo
                                      , ICurrentUserService user
                                      , IDateTimeProvider clock
                                      , IStorageService storageService
                                      , IMoMAttachmentRepository momatachmentRepository
                                      , IUnitOfWork uow
                                      , IOutlookService outlook
                                      , IEventBus eventBus
                                      , IMeetingNotificationRepository meetNotification
                                      , IPdfGenerator pdf
                                      , IOutboxService outbox)
        {
            _momRepo = monRepo;
            _user = user;
            _clock = clock;
            _storage = storageService;
            _momatachmentRepository = momatachmentRepository;
            _uow = uow;
            _outlook = outlook;
            _eventBus = eventBus;
            _meetNotification = meetNotification;
            _pdf = pdf;
            _outbox = outbox;
        }

        public async Task<Unit> Handle(ApproveMoMCommand req, CancellationToken ct)
        {
            //var mom = await _repo.GetByIdAsync(req.MoMId);
            //if (mom == null)
            //{
            //    throw new MoMNotFoundException(nameof(MinutesOfMeeting), req.MoMId);
            //}

            //if (mom.Status == MoMStatus.Approved)
            //    throw new DomainException("MoM already approved");
            //if (mom.Status != MoMStatus.PendingApproval)
            //    throw new DomainException("Only pending MoMs can be approved.");

            //mom.Status = MoMStatus.Approved;
            //mom.ApprovedAt = _clock.UtcNow;
            //mom.ApprovedBy = _user.UserId;

            //// create official PDF (placeholder): aggregate latest version content
            //var latest = mom.Versions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();
            //var pdfBytes = GeneratePdfFromHtmlOrText(latest?.Content ?? ""); // you must implement PDF generator in infra
            //var fileName = $"MoM_{mom.MeetingId}_{mom.VersionNumber}.pdf";
            //var storagePath = await _storage.SaveFileAsync(pdfBytes, fileName, "application/pdf", ct);

            //// store reference in Attachment table
            //var attachment = new MoMAttachment
            //{
            //    Id = Guid.NewGuid(),
            //    MoMId = mom.Id,
            //    FileName = fileName,
            //    StoragePath = storagePath,
            //    ContentType = "application/pdf",
            //    UploadedAt = _clock.UtcNow,
            //    UploadedBy = _user.UserId,
            //};
            //await _momatachmentRepository.AddAsync(attachment);

            //await _repo.UpdateAsync(mom);
            //await _uow.SaveChangesAsync(ct);

            //// optionally sync with Outlook meeting item to attach the PDF
            //await _outlook.SyncMeetingAttachmentAsync(mom.MeetingId, storagePath, ct);

            //await _eventBus.PublishAsync(new MoMApprovedEvent(mom.Id, mom.MeetingId, mom.ApprovedBy.Value, mom.ApprovedAt.Value, DateTime.Now), ct);

            //// Queue notification to all members (Outbox pattern via MeetingNotification table)
            //var notif = new MeetingNotification
            //{
            //    Id = Guid.NewGuid(),
            //    MeetingId = mom.MeetingId,
            //    NotificationType = NotificationType.MoMApproved,
            //    PayloadJson = JsonSerializer.Serialize(new { MoMId = mom.Id, Version = mom.VersionNumber, StoragePath = storagePath }),
            //    CreatedAt = _clock.UtcNow,
            //    Processed = false
            //};
            //await _meetNotification.AddAsync(notif);
            //await _uow.SaveChangesAsync(ct);

            //return Unit.Value;


            var mom = await _momRepo.GetByIdAsync(req.MoMId);
            if (mom == null)
            {
                throw new MoMNotFoundException(nameof(MinutesOfMeeting), req.MoMId);
            }


            mom.Approve(_clock.UtcNow, _user.UserId);


            var latest = mom.Versions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();
            var html = latest?.Content ?? string.Empty;


            var pdfBytes = _pdf.GeneratePdfFromHtml(html);
            var fileName = $"MoM_{mom.MeetingId}_{mom.VersionNumber}.pdf";
            var storagePath = await _storage.SaveFileAsync(pdfBytes, fileName, "application/pdf", ct);


            var attachment = new MoMAttachment(Guid.NewGuid(), mom.Id, fileName, storagePath, "application/pdf", _clock.UtcNow, _user.UserId);
            mom.AddAttachment(attachment);


            await _momRepo.UpdateAsync(mom);


            // Add outbox message for integration work (Outlook sync + notifications + bus publish)
            var outboxPayload = JsonSerializer.Serialize(new
            {
                Type = "MoMApproved",
                Data = new { MoMId = mom.Id, MeetingId = mom.MeetingId, StoragePath = storagePath }
            });


            await _outbox.EnqueueAsync(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow,
                Type = "MoMApproved",
                Payload = outboxPayload,
                Processed = false
            }, ct);


            await _uow.SaveChangesAsync(ct);


            // Clear domain events if you used them; domain events were also placed in outbox above.
            mom.ClearEvents();


            return Unit.Value;


        }
    }
}
