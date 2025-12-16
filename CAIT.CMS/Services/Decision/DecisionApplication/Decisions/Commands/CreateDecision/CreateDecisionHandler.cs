namespace DecisionApplication.Decisions.Commands.CreateDecision
{
    public sealed class CreateDecisionHandler
        : ICommandHandler<CreateDecisionCommand, CreateDecisionResult>
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateDecisionHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CreateDecisionResult> Handle(
            CreateDecisionCommand command,
            CancellationToken cancellationToken)
        {
            var decision = CreateNewDecision(command.Decision);

            _dbContext.Decisions.Add(decision);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CreateDecisionResult(decision.Id.Value);
        }

        private static Decision CreateNewDecision(CreateDecisionDto dto)
        {
            return Decision.Create(
                id: DecisionId.Of(Guid.NewGuid()),
                title: DecisionTitle.Of(dto.Title),
                text: DecisionText.Of(dto.TextArabic, dto.TextEnglish),
                meetingId: MeetingId.Of(dto.MeetingId),
                type: dto.Type,
                agendaItemId: dto.AgendaItemId is null
                    ? null
                    : AgendaItemId.Of(dto.AgendaItemId.Value)
            );
        }
    }
}
