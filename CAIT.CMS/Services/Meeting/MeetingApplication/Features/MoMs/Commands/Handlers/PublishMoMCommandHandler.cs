using MediatR;
using MeetingApplication.Common.CurrentUser;
using MeetingApplication.Common.DateTimeProvider;
using MeetingApplication.Exceptions;
using MeetingApplication.Features.MoMs.Commands.Models;
using MeetingApplication.Integrations;
using MeetingApplication.Interfaces;
using MeetingCore.Entities;
using MeetingCore.Enums;
using MeetingCore.Events;
using MeetingCore.Repositories;
using System.Text.Json;

namespace MeetingApplication.Features.MoMs.Commands.Handlers
{
    public class PublishMoMCommandHandler : IRequestHandler<PublishMoMCommand, string>
    {
        private readonly IMoMRepository _momRepo;
        private readonly IMoMAttachmentRepository _moMAttachmentRepository;
        private readonly IOutlookService _outlook;
        private readonly IEventBus _bus;
        private readonly IDateTimeProvider _clock;
        private readonly ICurrentUserService _user;
        private readonly IStorageService _storage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMeetingNotificationRepository _meetingNotificationRepository;

        public PublishMoMCommandHandler(IMoMRepository momRepo
                                      , IOutlookService outlook
                                      , IEventBus bus
                                      , IDateTimeProvider clock
                                      , ICurrentUserService user
                                      , IStorageService storage
                                      , IMoMAttachmentRepository moMAttachmentRepository
                                      , IUnitOfWork unitOfWork
                                      , IMeetingNotificationRepository meetingNotificationRepository)
        {
            _momRepo = momRepo;
            _outlook = outlook;
            _bus = bus;
            _clock = clock;
            _user = user;
            _storage = storage;
            _moMAttachmentRepository = moMAttachmentRepository;
            _unitOfWork = unitOfWork;
            _meetingNotificationRepository = meetingNotificationRepository;
        }

        public async Task<string> Handle(PublishMoMCommand req, CancellationToken ct)
        {
            var mom = await _momRepo.GetByIdAsync(req.MoMId);
            if (mom == null)
            {
                throw new MoMNotFoundException(nameof(MinutesOfMeeting), req.MoMId);
            }

            if (mom.Status != MoMStatus.Approved)
                throw new DomainException("Only approved MoMs can be published.");

            // create final PDF if not exists - search attachments
            var existingPdf = await _moMAttachmentRepository.GetPdfByMoMIdAsync(mom.Id, ct);
            string storagePath;
            if (existingPdf != null)
                storagePath = existingPdf.StoragePath;
            else
            {
                var latest = mom.Versions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();
                var pdf = GeneratePdfFromHtmlOrText(latest?.Content ?? "");
                var filename = $"MoM_{mom.MeetingId}_{mom.VersionNumber}.pdf";
                storagePath = await _storage.SaveFileAsync(pdf, filename, "application/pdf", ct);
                var attachment = new MoMAttachment(Guid.NewGuid(), mom.Id, filename, storagePath, "application/pdf", _clock.UtcNow, _user.UserId);
                await _moMAttachmentRepository.AddAsync(attachment);
                await _unitOfWork.SaveChangesAsync(ct);
            }

            mom.Status = MoMStatus.Published;
            //mom.DistributedAt = _clock.UtcNow;
            await _momRepo.UpdateAsync(mom);
            await _unitOfWork.SaveChangesAsync(ct);

            // update Outlook meeting with link

            //await _outlook.UpdateMeetingWithMinutesLinkAsync(mom.MeetingId, storagePath, ct);

            // queue notifications via Outbox
            var notif = new MeetingNotification
            {
                Id = Guid.NewGuid(),
                MeetingId = mom.MeetingId,
                NotificationType = NotificationType.MoMApproved,
                PayloadJson = JsonSerializer.Serialize(new { MoMId = mom.Id, StoragePath = storagePath }),
                CreatedAt = _clock.UtcNow,
                Processed = false
            };
            await _meetingNotificationRepository.AddAsync(notif);
            await _unitOfWork.SaveChangesAsync(ct);

            await _bus.PublishAsync(new MoMPublishedEvent(mom.Id, mom.MeetingId, _user.UserId, _clock.UtcNow, storagePath), ct);

            // return public link (or storage path)
            return storagePath;
        }

        private byte[] GeneratePdfFromHtmlOrText(string content)
        {
            return System.Text.Encoding.UTF8.GetBytes(content);
        }
    }

}
