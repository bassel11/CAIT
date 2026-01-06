using MassTransit;

namespace MeetingInfrastructure.Messaging.Consumers
{
    public class PermissionUpdateConsumerDefinition : ConsumerDefinition<PermissionUpdateConsumer>
    {
        public PermissionUpdateConsumerDefinition()
        {
            EndpointName = "meeting-permission-update";
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<PermissionUpdateConsumer> consumerConfigurator, IRegistrationContext context)
        {

            // إعادة المحاولة 3 مرات في حال الفشل
            endpointConfigurator.UseMessageRetry(r => r.Interval(3, 1000));
        }
    }
}
