using BuildingBlocks.Contracts.Decision.IntegrationEvents;
using DecisionCore.Events;

namespace DecisionApplication.Extensions
{
    public static class DecisionExtensions
    {
        public static DecisionCreatedIntegrationEvent
            ToDecisionCreatedIntegrationEvent(
                this DecisionCreatedEvent domainEvent)
        {
            return new DecisionCreatedIntegrationEvent(
                DecisionId: domainEvent.DecisionId.Value,
                MeetingId: domainEvent.MeetingId.Value,
                Title: domainEvent.Title.Value,
                TextArabic: domainEvent.TextArabic,
                TextEnglish: domainEvent.TextEnglish
            );
        }

        public static DecisionUpdatedIntegrationEvent
            ToDecisionUpdatedIntegrationEvent(
                this DecisionUpdatedEvent domainEvent)
        {
            return new DecisionUpdatedIntegrationEvent(
                DecisionId: domainEvent.DecisionId.Value,
                Title: domainEvent.Title.Value,
                TextArabic: domainEvent.TextArabic,
                TextEnglish: domainEvent.TextEnglish,
                Type: domainEvent.Type.ToString()
            );
        }

        public static DecisionDeletedIntegrationEvent
            ToDecisionDeletedIntegrationEvent(
                this DecisionDeletedEvent domainEvent)
        {
            return new DecisionDeletedIntegrationEvent(
                DecisionId: domainEvent.DecisionId.Value,
                Title: domainEvent.Title.Value
            );
        }
    }
}
