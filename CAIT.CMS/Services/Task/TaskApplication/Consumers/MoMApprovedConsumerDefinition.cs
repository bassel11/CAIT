using MassTransit;

namespace TaskApplication.Consumers
{
    public class MoMApprovedConsumerDefinition : ConsumerDefinition<MoMApprovedConsumer>
    {
        public MoMApprovedConsumerDefinition()
        {

            EndpointName = "task-mom-approved";
            ConcurrentMessageLimit = 10;
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<MoMApprovedConsumer> consumerConfigurator,
            IRegistrationContext context)
        {
            // إعدادات المتانة (Resilience)
            endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

            // يمكنك إضافة إعدادات أخرى هنا مثل UseInMemoryOutbox إذا لزم الأمر
        }
    }
}
