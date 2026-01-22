using MeetingCore.Enums.AttendanceEnums;
using MeetingCore.Enums.MeetingEnums;
using MeetingCore.Events.AttendanceEvents;
using MeetingCore.Events.MeetingEvents;
using MeetingCore.ValueObjects.AgendaItemVO;
using MeetingCore.ValueObjects.AIGeneratedContentVO;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingCore.Entities
{
    public class Meeting : Aggregate<MeetingId>
    {
        // ================= البيانات الأساسية =================
        public CommitteeId CommitteeId { get; private set; } = default!;
        public MeetingTitle Title { get; private set; } = default!;
        public string? Description { get; private set; }


        // التوقيت (دائماً UTC) والمكان
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public TimeZoneId TimeZone { get; private set; } = TimeZoneId.Utc;
        public MeetingLocation Location { get; private set; } = default!;

        // الحالة والتحكم
        public MeetingStatus Status { get; private set; } = MeetingStatus.Draft;
        public string? CancellationReason { get; private set; } = default!;

        // التكامل (Microsoft Graph)
        public string? TeamsLink { get; private set; }
        public string? OutlookEventId { get; private set; }

        // التكرار
        public RecurrencePattern Recurrence { get; private set; }

        // Concurrency Control
        public byte[] RowVersion { get; set; } = default!;

        // ================= العلاقات (Collections) =================
        private readonly List<AgendaItem> _agendaItems = new();
        public IReadOnlyCollection<AgendaItem> AgendaItems => _agendaItems.AsReadOnly();

        private readonly List<Attendance> _attendances = new();
        public IReadOnlyCollection<Attendance> Attendances => _attendances.AsReadOnly();

        private readonly List<AIGeneratedContent> _aiContents = new();
        public IReadOnlyCollection<AIGeneratedContent> AIContents => _aiContents.AsReadOnly();

        // علاقة 1-to-1 مع المحضر
        public MinutesOfMeeting? Minutes { get; private set; }

        // ================= البناء (Construction) =================
        private Meeting() { } // EF Core

        public Meeting(
            MeetingId id,
            CommitteeId committeeId,
            MeetingTitle title,
            string? description,
            DateTime startDate,
            DateTime endDate,
            TimeZoneId timeZone,
            MeetingLocation location,
            RecurrencePattern recurrence,
            string? createdBy)
        {
            Id = id ?? throw new DomainException("MeetingId is required.");
            CommitteeId = committeeId ?? throw new DomainException("CommitteeId is required.");
            Title = title ?? throw new DomainException("Title is required.");
            StartDate = startDate;
            EndDate = endDate;
            TimeZone = timeZone ?? TimeZoneId.Utc;
            Location = location ?? throw new DomainException("Location is required.");
            Description = description;
            Recurrence = recurrence ?? RecurrencePattern.None;

            Status = MeetingStatus.Draft;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
        }

        // ================= Factory Method =================
        public static Meeting Create(
            MeetingId id,
            CommitteeId committeeId,
            MeetingTitle title,
            string? description,
            DateTime startDate,
            DateTime endDate,
            TimeZoneId timeZone,
            MeetingLocation location,
            RecurrencePattern recurrence,
            string? createdBy)
        {
            if (endDate <= startDate)
                throw new DomainException("Meeting end date must be after start date.");

            if (location.Type == LocationType.Physical && string.IsNullOrWhiteSpace(location.RoomName) && string.IsNullOrWhiteSpace(location.Address))
                throw new DomainException("Physical meetings must have a location.");

            var meeting = new Meeting
            {
                Id = id,
                CommitteeId = committeeId,
                Title = title,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                TimeZone = timeZone,
                Location = location,
                Recurrence = recurrence,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };


            meeting.AddDomainEvent(new MeetingCreatedEvent(
                meeting.Id.Value,
                meeting.CommitteeId.Value,
                meeting.Title.Value,
                meeting.StartDate,
                meeting.EndDate,
                meeting.TimeZone.Value,
                createdBy, // تحويل الـ Audit String إلى Guid
                meeting.CreatedAt
    ));
            return meeting;
        }


        public void UpdateDetails(
            MeetingTitle title,
            string? description,
            MeetingLocation location,
            string modifiedBy)
        {
            // 1. التحقق من القوانين (Invariants)
            // لا يمكن تعديل تفاصيل اجتماع ملغي أو مكتمل
            if (Status == MeetingStatus.Cancelled || Status == MeetingStatus.Completed)
                throw new DomainException("Cannot update details of a finished meeting.");

            // 2. تطبيق التغييرات
            Title = title;
            Description = description;
            Location = location;

            // 3. تحديث التدقيق
            LastModifiedBy = modifiedBy;
            LastTimeModified = DateTime.UtcNow;

            // 4. إطلاق حدث (اختياري، مفيد لتحديث الـ Read Models)
            // AddDomainEvent(new MeetingDetailsUpdatedEvent(Id, Title.Value, ...));
        }

        // ================= منطق العمل (Domain Behaviors) =================

        public void Schedule()
        {
            if (Status != MeetingStatus.Draft)
                throw new DomainException("Only draft meetings can be scheduled.");

            if (!_agendaItems.Any())
                throw new DomainException("Cannot schedule a meeting without an Agenda.");

            Status = MeetingStatus.Scheduled;
            LastTimeModified = DateTime.UtcNow;
            //LastModifiedBy = userId;

            AddDomainEvent(new MeetingScheduledEvent(
                Id,
                CommitteeId,
                Title.Value,
                StartDate,
                EndDate,
                _attendances.Select(a => a.UserId.Value).ToList()));
        }

        // داخل كلاس Meeting
        public void Complete(string userId)
        {
            // 1. التحقق من القوانين (Invariants)
            if (Status == MeetingStatus.Completed)
                throw new DomainException("Meeting is already completed.");

            if (Status == MeetingStatus.Cancelled)
                throw new DomainException("Cannot complete a cancelled meeting.");

            // يمكن إضافة شرط: لا يمكن إنهاء اجتماع مستقبلي (يجب أن يكون تاريخه قد حان)
            // if (DateTime.UtcNow < StartDate) throw ...

            // 2. تغيير الحالة
            Status = MeetingStatus.Completed;

            // 3. تحديث التدقيق
            LastModifiedBy = userId;
            LastTimeModified = DateTime.UtcNow;

            // 4. إطلاق حدث (مهم جداً هنا لإنشاء مسودة المحضر تلقائياً مثلاً)
            AddDomainEvent(new MeetingCompletedEvent(Id.Value, DateTime.UtcNow));
        }
        public void Reschedule(DateTime newStart, DateTime newEnd, string userId)
        {
            if (Status == MeetingStatus.Cancelled || Status == MeetingStatus.Completed)
                throw new DomainException("Cannot reschedule a finished meeting.");

            StartDate = newStart;
            EndDate = newEnd;
            Status = MeetingStatus.Rescheduled;
            LastModifiedBy = userId;
            LastTimeModified = DateTime.UtcNow;

            AddDomainEvent(new MeetingRescheduledEvent(Id, newStart, newEnd, OutlookEventId));
        }

        public void Cancel(string reason, string? cancelledBy = null)
        {
            if (Status == MeetingStatus.Completed)
                throw new DomainException("Cannot cancel a completed meeting.");

            if (Status == MeetingStatus.Cancelled)
                return;

            Status = MeetingStatus.Cancelled;
            CancellationReason = reason;
            LastModifiedBy = cancelledBy;
            LastTimeModified = DateTime.UtcNow;

            AddDomainEvent(new MeetingCancelledEvent(
                Id,
                reason,
                OutlookEventId,
                _attendances.Select(a => a.UserId.Value).ToList()
            ));
        }


        public void UpdateIntegrationInfo(string outlookEventId, string teamsLink)
        {
            OutlookEventId = outlookEventId;
            TeamsLink = teamsLink;
        }

        // إضافة الحضور والأجندة
        public void AddAttendee(
             UserId userId,
             AttendanceRole role,
             VotingRight votingRight)
        {
            if (_attendances.Any(a => a.UserId == userId))
                return;

            _attendances.Add(
              new Attendance(
                    Id,
                    userId,
                    role,
                    votingRight));
        }

        // داخل كلاس Meeting

        public void RemoveAttendee(UserId userId)
        {
            var attendance = _attendances.FirstOrDefault(a => a.UserId == userId);
            if (attendance == null) throw new DomainException("Attendee not found.");

            // لا يمكن حذف حضور قام بتسجيل الدخول بالفعل (Business Rule)
            if (attendance.AttendanceStatus != AttendanceStatus.None)
                throw new DomainException("Cannot remove an attendee who has already checked in.");

            _attendances.Remove(attendance);
            LastTimeModified = DateTime.UtcNow;

            AddDomainEvent(new MeetingAttendeeRemovedEvent(Id.Value, userId.Value));
        }

        public void ConfirmRSVP(UserId userId, RSVPStatus status)
        {
            var attendance = _attendances.FirstOrDefault(a => a.UserId == userId);
            if (attendance == null) throw new DomainException("Attendee not found in this meeting.");

            attendance.ConfirmRSVP(status);
            LastTimeModified = DateTime.UtcNow;

            AddDomainEvent(new MeetingAttendeeRSVPedEvent(
                Id.Value,
                userId.Value,
                status,
                DateTime.UtcNow
            ));
        }

        public void CheckInAttendee(UserId userId, bool isRemote)
        {
            // تحقق من أن الاجتماع في حالة تسمح بتسجيل الدخول (مثلاً Scheduled أو Started)
            if (Status == MeetingStatus.Draft || Status == MeetingStatus.Cancelled)
                throw new DomainException("Cannot check-in to a draft or cancelled meeting.");

            var attendance = _attendances.FirstOrDefault(a => a.UserId == userId);

            // إذا كان "Walk-in" (غير مدعو)، يجب إضافته أولاً (حسب قواعد العمل).
            // هنا سنفترض الصرامة: يجب أن يكون مدعواً.
            if (attendance == null) throw new DomainException("User is not listed as an attendee.");

            attendance.CheckIn(isRemote);
            LastTimeModified = DateTime.UtcNow;

            var status = isRemote ? AttendanceStatus.Remote : AttendanceStatus.Present;

            AddDomainEvent(new MeetingAttendeeCheckedInEvent(
                    Id.Value,
                    userId.Value,
                    status,      // تمرير الحالة كـ Enum
                    isRemote,    // تمرير الـ flag (إذا أضفته للحدث كما اقترحت أعلاه)
                    DateTime.UtcNow
                ));
        }

        public void BulkCheckIn(List<(UserId UserId, AttendanceStatus Status)> entries)
        {
            var now = DateTime.UtcNow; // توحيد التوقيت للعملية كاملة

            // قائمة لتجميع التغييرات التي تمت بنجاح لإرسالها في الحدث
            var changesList = new List<BulkCheckInItem>();

            foreach (var entry in entries)
            {
                var attendance = _attendances.FirstOrDefault(a => a.UserId == entry.UserId);

                // نتجاوز من ليس موجوداً في القائمة (أو يمكن إضافته هنا لو أردت منطق Walk-in)
                if (attendance != null)
                {
                    // 1. التحديث الفعلي للحالة باستخدام التابع الداخلي
                    attendance.SetStatus(entry.Status, now);

                    // 2. إضافة التغيير للقائمة المؤقتة
                    changesList.Add(new BulkCheckInItem(entry.UserId.Value, entry.Status));
                }
            }

            // 3. نتحقق هل تم تعديل أي شيء فعلاً؟
            if (changesList.Any())
            {
                LastTimeModified = now;

                // 4. إطلاق حدث واحد يحتوي على كل التفاصيل
                AddDomainEvent(new MeetingAttendeesBulkCheckedInEvent(
                    Id.Value,
                    changesList,
                    now
                ));
            }
        }
        public void AddAgendaItem(
            AgendaItemTitle title,
            Duration? allocatedTime,
            SortOrder sortOrder,
            PresenterId? presenterId,
            string? description = null)
        {
            if (_agendaItems.Any(a => a.SortOrder == sortOrder))
                throw new DomainException("Duplicate agenda item order.");

            _agendaItems.Add(new AgendaItem(
                Id,
                title,
                allocatedTime,
                sortOrder,
                presenterId,
                description));

            LastTimeModified = DateTime.UtcNow;
        }

        public void UpdateAgendaItem(
            AgendaItemId itemId,
            AgendaItemTitle title,
            string? description,
            SortOrder sortOrder,
            Duration? allocatedTime,
            PresenterId? presenterId)
        {
            var item = _agendaItems.FirstOrDefault(x => x.Id == itemId);
            if (item == null) throw new DomainException("Agenda item not found in this meeting.");

            // تحديث البند
            item.Update(title, description, sortOrder, allocatedTime, presenterId);

            LastTimeModified = DateTime.UtcNow;
        }

        public void RemoveAgendaItem(AgendaItemId itemId)
        {
            var item = _agendaItems.FirstOrDefault(x => x.Id == itemId);
            if (item == null) throw new DomainException("Agenda item not found.");

            _agendaItems.Remove(item);
            LastTimeModified = DateTime.UtcNow;
        }

        public void LogAIGeneration(
            AIContentType type,
            string prompt,
            string result,
            string modelName,
            UserId userId)
        {
            // يمكن وضع قيود هنا، مثلاً: لا يمكن توليد أجندة لاجتماع منتهي

            var aiContent = new AIGeneratedContent(
                Id,
                type,
                prompt,
                result,
                modelName,
                userId
            );

            _aiContents.Add(aiContent);
        }

        // دالة لتطبيق النص المقترح (مثلاً نسخ الأجندة المقترحة إلى الأجندة الحقيقية)
        public void ApplyAIContent(AIContentId contentId)
        {
            var content = _aiContents.FirstOrDefault(x => x.Id == contentId);
            if (content == null) throw new DomainException("AI Content not found.");

            // نضع علامة أنه تم الاستفادة منه
            content.MarkAsApplied();

            // هنا يمكن تنفيذ المنطق، مثلاً:
            // if (content.ContentType == AIContentType.AgendaDraft) { ... نسخ النصوص ... }
        }

    }
}
