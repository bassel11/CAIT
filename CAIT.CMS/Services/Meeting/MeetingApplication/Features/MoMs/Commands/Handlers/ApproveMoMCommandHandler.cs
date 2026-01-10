using BuildingBlocks.Contracts.Integration;
using BuildingBlocks.Contracts.Notifications;
using BuildingBlocks.Contracts.Outlook;
using BuildingBlocks.Shared.Exceptions;
using MassTransit;
using MediatR;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingApplication.Integrations;
using MeetingApplication.Interfaces;
using MeetingCore.Entities;
using MeetingCore.Repositories;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class ApproveMoMCommandHandler : IRequestHandler<ApproveMoMCommand, Unit>
    {
        private readonly IMoMRepository _momRepo;
        private readonly ICurrentUserService _user;
        private readonly IDateTimeProvider _clock;
        private readonly IStorageService _storage;
        private readonly IMoMAttachmentRepository _momAttachmentRepository;
        private readonly IUnitOfWork _uow;
        private readonly IOutlookService _outlook;
        private readonly IMeetingNotificationRepository _meetNotification;
        private readonly IPdfGenerator _pdf;
        private readonly IPublishEndpoint _publishEndpoint; // MassTransit Publish

        public ApproveMoMCommandHandler(
            IMoMRepository momRepo,
            ICurrentUserService user,
            IDateTimeProvider clock,
            IStorageService storageService,
            IMoMAttachmentRepository momAttachmentRepository,
            IUnitOfWork uow,
            IOutlookService outlook,
            IMeetingNotificationRepository meetNotification,
            IPdfGenerator pdf,
            IPublishEndpoint publishEndpoint)
        {
            _momRepo = momRepo;
            _user = user;
            _clock = clock;
            _storage = storageService;
            _momAttachmentRepository = momAttachmentRepository;
            _uow = uow;
            _outlook = outlook;
            _meetNotification = meetNotification;
            _pdf = pdf;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Unit> Handle(ApproveMoMCommand req, CancellationToken ct)
        {
            var mom = await _momRepo.GetMoMByIdAsync(req.MoMId);
            if (mom == null)
                throw new NotFoundException(nameof(MinutesOfMeeting), req.MoMId);

            mom.Approve(_clock.UtcNow, _user.UserId);

            var latest = mom.Versions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();
            var html = latest?.Content ?? "";

            var pdfBytes = _pdf.GeneratePdfFromHtml(html);
            var fileName = $"MoM_{mom.MeetingId}_{mom.VersionNumber}.pdf";
            var path = await _storage.SaveFileAsync(pdfBytes, fileName, "application/pdf", ct);

            var attachment = new MoMAttachment(Guid.NewGuid(), mom.Id, fileName, path, "application/pdf", _clock.UtcNow, _user.UserId);

            await _momAttachmentRepository.AddMoMAttachmentAsync(attachment);
            mom.MoMAttachments.Add(attachment);

            await _momRepo.UpdateMoMAsync(mom);

            // نشر الأحداث باستخدام MassTransit domain events
            // داخل ApproveMoMCommandHandler.Handle بعد mom.Events موجودة و قبل ClearEvents()

            //foreach (var domainEvt in mom.Events)
            //{
            //    // publish the domain event itself (so other services listening for domain events get it)
            //    await _publishEndpoint.Publish(domainEvt, ct);

            //    // map to strongly-typed audit contract when possible
            //    if (domainEvt is MeetingCore.Events.MoMApprovedEvent approved)
            //    {
            //        await _publishEndpoint.Publish<IAuditLogCreated>(new
            //        {
            //            EventId = approved.EventId,
            //            UserId = approved.ApprovedBy.ToString(),
            //            ServiceName = "MeetingService",
            //            EntityName = "MinutesOfMeeting",
            //            ActionType = "Approve",
            //            PrimaryKey = approved.MoMId.ToString(),
            //            OldValues = (string?)null,
            //            NewValues = (string?)null,
            //            Timestamp = approved.OccurredAt
            //        }, ct);
            //    }
            //    // else-if for other concrete domain events...
            //}


            // نشر رسائل Integration / Notifications  events
            await _publishEndpoint.Publish(new AttachMoMToOutlookEvent(mom.MeetingId, path), ct);

            await _publishEndpoint.Publish(new MoMApprovedNotification(
                mom.Id, mom.MeetingId, "bassel.as19@gmail.com", "MoM Approved", "Your MoM has been approved"), ct);

            await _publishEndpoint.Publish(new MoMApprovedIntegrationEvent(mom.Id, mom.MeetingId), ct);


            //mom.ClearEvents();

            return Unit.Value;
        }
    }
}



//using MediatR;
//using MeetingApplication.Common.CurrentUser;
//using MeetingApplication.Common.DateTimeProvider;
//using MeetingApplication.Exceptions;
//using MeetingApplication.Features.MoMs.Commands.Models;
//using MeetingApplication.Integrations;
//using MeetingApplication.Interfaces;
//using MeetingCore.Entities;
//using MeetingCore.Repositories;

//namespace MeetingApplication.Features.MoMs.Commands.Handlers
//{
//    public class ApproveMoMCommandHandler : IRequestHandler<ApproveMoMCommand, Unit>
//    {
//        private readonly IMoMRepository _momRepo;
//        private readonly ICurrentUserService _user;
//        private readonly IDateTimeProvider _clock;
//        private readonly IStorageService _storage;
//        private readonly IMoMAttachmentRepository _momatachmentRepository;
//        private readonly IUnitOfWork _uow;
//        private readonly IOutlookService _outlook;
//        private readonly IEventBus _eventBus;
//        private readonly IMeetingNotificationRepository _meetNotification;
//        private readonly IPdfGenerator _pdf;
//        private readonly IOutboxService _outbox;


//        public ApproveMoMCommandHandler(IMoMRepository monRepo
//                                      , ICurrentUserService user
//                                      , IDateTimeProvider clock
//                                      , IStorageService storageService
//                                      , IMoMAttachmentRepository momatachmentRepository
//                                      , IUnitOfWork uow
//                                      , IOutlookService outlook
//                                      , IEventBus eventBus
//                                      , IMeetingNotificationRepository meetNotification
//                                      , IPdfGenerator pdf
//                                      , IOutboxService outbox)
//        {
//            _momRepo = monRepo;
//            _user = user;
//            _clock = clock;
//            _storage = storageService;
//            _momatachmentRepository = momatachmentRepository;
//            _uow = uow;
//            _outlook = outlook;
//            _eventBus = eventBus;
//            _meetNotification = meetNotification;
//            _pdf = pdf;
//            _outbox = outbox;
//        }

//        public async Task<Unit> Handle(ApproveMoMCommand req, CancellationToken ct)
//        {

//            // await _uow.BeginTransactionAsync(ct);

//            var mom = await _momRepo.GetMoMByIdAsync(req.MoMId);
//            if (mom == null)
//                throw new MoMNotFoundException(nameof(MinutesOfMeeting), req.MoMId);

//            mom.Approve(_clock.UtcNow, _user.UserId);

//            var latest = mom.Versions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();
//            var html = latest?.Content ?? "";

//            var pdfBytes = _pdf.GeneratePdfFromHtml(html);
//            var fileName = $"MoM_{mom.MeetingId}_{mom.VersionNumber}.pdf";

//            var path = await _storage.SaveFileAsync(pdfBytes, fileName, "application/pdf", ct);

//            //mom.AddAttachment(new MoMAttachment(Guid.NewGuid(), mom.Id, fileName, path, "application/pdf", _clock.UtcNow, _user.UserId));

//            var attachment = new MoMAttachment(Guid.NewGuid(), mom.Id, fileName, path, "application/pdf", _clock.UtcNow, _user.UserId);

//            // 1) أضف إلى DbSet مباشرة (إذا لديك repo method)
//            await _momatachmentRepository.AddMoMAttachmentAsync(attachment); // هذه تضيف إلى DbContext (Scoped)

//            // 2) ثم عدّل mom.Versions أو mom.MoMAttachments إن أردت، لكن لا تفعل UpdateMoMAsync الذي يعيد تحميل الكيان
//            mom.MoMAttachments.Add(attachment);

//            await _momRepo.UpdateMoMAsync(mom);

//            foreach (var evt in mom.Events)
//            {
//                await _outbox.EnqueueAsync(evt.GetType().Name, evt, ct);
//            }

//            // OUTBOX MESSAGES
//            await _outbox.EnqueueAsync("Outlook:AttachMoM", new
//            {
//                MeetingId = mom.MeetingId,
//                Url = path
//            }, ct);

//            await _outbox.EnqueueAsync("Notification:MoMPublished", new
//            {
//                to = "bassel.as19@gmail.com",
//                subject = "MoM Approved",
//                body = "Your MoM has been approved"
//            }, ct);

//            await _outbox.EnqueueAsync("Integration:MoM.Approved", new
//            {
//                momId = mom.Id,
//                meetingId = mom.MeetingId
//            }, ct);

//            // await _uow.SaveChangesAsync(ct);
//            mom.ClearEvents();

//            return Unit.Value;


//        }
//    }
//}


////// old version 

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
