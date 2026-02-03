namespace DecisionCore.Entities
{
    public class Decision : Aggregate<DecisionId>
    {
        private readonly List<Vote> _votes = new();
        public IReadOnlyList<Vote> Votes => _votes.AsReadOnly();

        public DecisionTitle Title { get; private set; } = default!;
        public DecisionText Text { get; private set; } = default!;
        public MeetingId MeetingId { get; private set; } = default!;
        public MoMId? MoMId { get; private set; } = default!;
        public AgendaItemId? AgendaItemId { get; private set; }
        public VotingDeadline? VotingDeadline { get; private set; }
        public DecisionType Type { get; private set; } = default!;
        public DecisionStatus Status { get; private set; } = DecisionStatus.Draft;


        private Decision() { } // For EF Core

        public static Decision Create(
            DecisionId id,
            DecisionTitle title,
            DecisionText text,
            MeetingId meetingId,
            DecisionType type,
            AgendaItemId? agendaItemId = null)
        {
            var decision = new Decision
            {
                Id = id,
                Title = title,
                Text = text,
                MeetingId = meetingId,
                Type = type,
                AgendaItemId = agendaItemId,
                Status = DecisionStatus.Draft
            };

            decision.AddDomainEvent(new DecisionCreatedEvent(
                decision.Id,
                decision.MeetingId,
                decision.Title,
                decision.Text.Arabic,
                decision.Text.English));

            return decision;
        }

        public void Update(
            DecisionTitle title,
            DecisionText text,
            DecisionType type,
            AgendaItemId? agendaItemId)
        {
            if (Status is DecisionStatus.Approved or DecisionStatus.Rejected)
                throw new DomainException("Cannot update a decision that has been finalized.");

            Title = title;
            Text = text;
            Type = type;
            AgendaItemId = agendaItemId;

            AddDomainEvent(new DecisionUpdatedEvent(
                Id,
                MeetingId,
                Title,
                Text.Arabic,
                Text.English,
                Type,
                AgendaItemId
            ));
        }

        public void Delete()
        {
            if (Status is DecisionStatus.Approved or DecisionStatus.Rejected)
                throw new DomainException("Cannot delete a decision that has been finalized.");

            AddDomainEvent(new DecisionDeletedEvent(Id, Title));
        }

        public void OpenVoting(VotingDeadline deadline)
        {
            if (Type != DecisionType.Voting)
                throw new DomainException("This decision type does not support voting.");

            VotingDeadline = deadline;
            Status = DecisionStatus.PendingVoting;

            AddDomainEvent(new DecisionVotingOpenedEvent(Id, VotingDeadline));
        }

        public void AddVote(Guid memberId, VoteType voteType)
        {
            if (Status != DecisionStatus.PendingVoting)
                throw new DomainException("Voting is not currently open for this decision.");

            if (VotingDeadline != null && VotingDeadline.IsPassed())
                throw new DomainException("The voting deadline has passed.");

            if (_votes.Any(v => v.MemberId == memberId))
                throw new DomainException("Member has already voted on this decision.");

            var vote = new Vote(Id, memberId, voteType);
            _votes.Add(vote);

            AddDomainEvent(new VoteAddedEvent(Id, memberId, voteType));
        }

        public void FinalizeDecision(Guid finalizedBy)
        {
            if (Status != DecisionStatus.PendingVoting)
                throw new DomainException("Decision cannot be finalized at this stage.");

            if (Type == DecisionType.Voting && !_votes.Any())
                throw new DomainException("Cannot finalize a voting decision without any votes.");

            var yesVotes = _votes.Count(v => v.Type == VoteType.Yes);
            var noVotes = _votes.Count(v => v.Type == VoteType.No);

            Status = yesVotes > noVotes
                ? DecisionStatus.Approved
                : DecisionStatus.Rejected;

            AddDomainEvent(new DecisionFinalizedEvent(Id, MeetingId, Status, finalizedBy));
        }

        #region 

        public static Decision CreateDecisionFromMoM(
            DecisionId id,
            DecisionTitle title,
            DecisionText text,
            MeetingId meetingId,
            MoMId momId) // المصدر
        {
            var decision = new Decision
            {
                Id = id,
                Title = title,
                Text = text,
                MeetingId = meetingId,

                // القرارات من المحضر عادة لا تحتاج تصويت، هي قرارات إدارية أو تم التصويت عليها شفهياً
                Type = DecisionType.ChairmanAuthority, // هام أو نوع جديد مثل Official

                // ✅ الحالة هنا يجب أن تكون معتمدة فوراً
                Status = DecisionStatus.Approved,

                // ربط بالمصدر
                MoMId = momId
            };

            // نطلق حدثاً مختلفاً (اختياري) أو نفس الحدث
            decision.AddDomainEvent(new DecisionCreatedEvent(
                decision.Id,
                decision.MeetingId,
                decision.Title,
                decision.Text.Arabic,
                decision.Text.English));

            return decision;
        }

        #endregion

    }
}
