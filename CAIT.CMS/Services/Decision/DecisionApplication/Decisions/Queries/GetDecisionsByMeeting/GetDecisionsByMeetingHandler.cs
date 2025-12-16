namespace DecisionApplication.Decisions.Queries.GetDecisionsByMeeting
{
    public class GetDecisionsByMeetingHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetDecisionsByMeetingQuery, GetDecisionsByMeetingResult>
    {
        public async Task<GetDecisionsByMeetingResult> Handle(GetDecisionsByMeetingQuery query, CancellationToken cancellationToken)
        {
            var decisions = await dbContext.Decisions
                .Include(d => d.Votes)
                .AsNoTracking()
                .Where(d => d.MeetingId == MeetingId.Of(query.MeetingId))
                .OrderBy(d => d.CreatedAt)
                .ToListAsync(cancellationToken);

            return new GetDecisionsByMeetingResult(decisions.ToDecisionDtoList());
        }
    }
}
