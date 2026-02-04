using MassTransit;

namespace DecisionApplication.Consumers
{
    public class MoMApprovedConsumerDefinition : ConsumerDefinition<MoMApprovedConsumer>
    {
        public MoMApprovedConsumerDefinition()
        {
            EndpointName = "decision-mom-approved";
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<MoMApprovedConsumer> consumerConfigurator,
            IRegistrationContext context)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        }
    }
}
