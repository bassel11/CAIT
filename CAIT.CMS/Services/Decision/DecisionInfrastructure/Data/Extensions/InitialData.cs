namespace DecisionInfrastructure.Data.Extensions
{
    internal class InitialData
    {
        // مثال للقرارات الأولية
        // قائمة بالقرارات الأولية فقط
        public static IEnumerable<Decision> Decisions
        {
            get
            {
                var decision1 = Decision.Create(
                    DecisionId.Of(Guid.NewGuid()),
                    DecisionTitle.Of("قرار رقم 1"),
                    DecisionText.Of("نص عربي 1", "English text 1"),
                    MeetingId.Of(Guid.NewGuid()),
                    DecisionType.Voting
                );

                var decision2 = Decision.Create(
                    DecisionId.Of(Guid.NewGuid()),
                    DecisionTitle.Of("قرار رقم 2"),
                    DecisionText.Of("نص عربي 2", "English text 2"),
                    MeetingId.Of(Guid.NewGuid()),
                    DecisionType.Consensus
                );

                var decision3 = Decision.Create(
                    DecisionId.Of(Guid.NewGuid()),
                    DecisionTitle.Of("قرار رقم 3"),
                    DecisionText.Of("نص عربي 3", "English text 3"),
                    MeetingId.Of(Guid.NewGuid()),
                    DecisionType.ChairmanAuthority
                );

                return new List<Decision> { decision1, decision2, decision3 };
            }
        }
    }
}
