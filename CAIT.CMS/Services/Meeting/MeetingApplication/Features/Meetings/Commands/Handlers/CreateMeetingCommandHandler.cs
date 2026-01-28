using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingApplication.Interfaces.Committee;
using MeetingCore.DomainServices;
using MeetingCore.Entities;
using MeetingCore.Enums.MeetingEnums;
using MeetingCore.Repositories;
using MeetingCore.ValueObjects;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Meetings.Commands.Handlers
{
    public class CreateMeetingCommandHandler : ICommandHandler<CreateMeetingCommand, Result<Guid>>
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICommitteeService _committeeService; // ✅ خدمة لجلب الأعضاء
        private readonly IMeetingOverlapDomainService _overlapService; // ✅ 1. تعريف الخدمة

        public CreateMeetingCommandHandler(
            IMeetingRepository meetingRepository,
            ICurrentUserService currentUserService,
            ICommitteeService committeeService,
            IMeetingOverlapDomainService overlapService)
        {
            _meetingRepository = meetingRepository;
            _currentUserService = currentUserService;
            _committeeService = committeeService;
            _overlapService = overlapService;
        }

        public async Task<Result<Guid>> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
        {

            // =========================================================
            // 1. تحويل Primitives إلى Value Objects (Domain Validation)
            // =========================================================
            var currentUserId = _currentUserService.UserId.ToString();
            var meetingId = MeetingId.Of(Guid.NewGuid());
            var committeeId = CommitteeId.Of(request.CommitteeId);
            var title = MeetingTitle.Of(request.Title);

            // التحقق من المنطقة الزمنية
            TimeZoneId timeZone;
            timeZone = TimeZoneId.Of(request.TimeZone);


            // بناء كائن الموقع
            var location = MeetingLocation.Create(
                request.LocationType,
                request.LocationRoom,
                request.LocationAddress,
                request.LocationOnlineUrl
            );

            // بناء كائن التكرار
            //  منطق قوي يحترم النوع أولاً
            // 1. القيمة الافتراضية: لا يوجد تكرار
            RecurrencePattern recurrence = RecurrencePattern.None;

            // 2. هل التكرار مفعل؟
            if (request.IsRecurring)
            {
                // A. الأولوية الأولى: النوع القياسي (Daily, Weekly, etc)
                // نتحقق أن القيمة موجودة وليست None
                if (request.RecurrenceType.HasValue && request.RecurrenceType != RecurrenceType.None)
                {
                    // نستخدم النمط البسيط وننظف الـ Rule (حتى لو أرسل الـ Frontend بيانات خاطئة هنا)
                    recurrence = RecurrencePattern.Simple(request.RecurrenceType.Value);
                }
                // B. الأولوية الثانية: القاعدة المخصصة (Custom Rule)
                // نصل هنا فقط إذا لم يكن هناك Type
                else if (!string.IsNullOrWhiteSpace(request.RecurrenceRule))
                {
                    // نستخدم القاعدة المخصصة
                    recurrence = RecurrencePattern.WithRule(request.RecurrenceRule);
                }
                // C. معالجة الخطأ: التكرار مفعل لكن لا يوجد نوع ولا قاعدة
                else
                {
                    throw new DomainException("Recurring meetings must have a valid Recurrence Type or Rule.");
                }
            }


            // =========================================================
            // ✅ الخطوة الجديدة: التحقق من تعارض القاعات قبل الإنشاء
            // =========================================================

            // نتحقق فقط إذا لم يكن الاجتماع متكرراً (التكرار يتطلب منطقاً أعقد، سنناقشه لاحقاً)
            // أو نتحقق من أول حدوث (First Occurrence) مبدئياً
            if (!request.IsRecurring)
            {
                await _overlapService.ValidateRoomAvailabilityAsync(
                    request.StartDate,
                    request.EndDate,
                    location,
                    null, // لا يوجد ID للاستثناء لأنه اجتماع جديد
                    cancellationToken
                );
            }
            else
            {
                // ملاحظة: للاجتماعات المتكررة، عادة نحجز الغرفة لأول موعد،
                // أما المواعيد المستقبلية فنتحقق منها عند توليدها (Expansion).
                // لكن للتبسيط الآن، نتحقق من أول موعد.
                await _overlapService.ValidateRoomAvailabilityAsync(
                   request.StartDate,
                   request.EndDate,
                   location,
                   null,
                   cancellationToken
               );
            }

            // =========================================================
            // 2. إنشاء الـ Aggregate Root باستخدام الـ Factory
            // =========================================================

            var meeting = Meeting.Create(
                meetingId,
                committeeId,
                title,
                request.Description,
                request.StartDate,
                request.EndDate,
                timeZone,
                location,
                recurrence,
                currentUserId // Audit User
            );


            // =========================================================
            // ✅ 3. (الجديد) جلب أعضاء اللجنة وإضافتهم تلقائياً
            // =========================================================

            if (request.AutoAddMembers)
            {
                try
                {
                    var committeeMembers = await _committeeService.GetActiveMembersAsync(committeeId.Value, cancellationToken);

                    if (committeeMembers != null && committeeMembers.Any())
                    {
                        foreach (var member in committeeMembers)
                        {
                            var userId = UserId.Of(member.UserId);
                            meeting.AddAttendee(
                                userId,
                                member.Role,
                                member.VotingRight
                            );
                        }
                    }
                    else
                    {
                        // تحذير أو خطأ حسب رغبتك، لكن يفضل السماح بذلك وإضافة الأعضاء يدوياً لاحقاً
                    }
                }
                catch (Exception ex)
                {
                    // سؤال معماري مهم: هل نوقف إنشاء الاجتماع إذا فشل جلب الأعضاء؟
                    // في الأنظمة المرنة: لا، نسجل تحذير ونكمل الإنشاء، والمستخدم يضيفهم يدوياً لاحقاً.
                    // _logger.LogWarning(ex, "Failed to auto-fetch committee members.");

                    // أما إذا كان العمل يتطلب صرامة تامة، اترك الـ Exception يوقف العملية.
                    throw;
                }
            }

            // =========================================================
            // 4. الحفظ وضمان الـ Transactionality
            // =========================================================

            // إضافة للذاكرة
            await _meetingRepository.AddAsync(meeting, cancellationToken);

            // الحفظ النهائي (مع Outbox Messages)
            await _meetingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(meeting.Id.Value, "Meeting created successfully.");

        }
    }
}
