using MassTransit;

namespace NotificationService.Consumers.Security
{
    public class PermissionUpdateConsumerDefinition : ConsumerDefinition<PermissionUpdateConsumer>
    {
        public PermissionUpdateConsumerDefinition()
        {
            EndpointName = "notification-permission-update";
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<PermissionUpdateConsumer> consumerConfigurator, IRegistrationContext context)
        {

            // إعادة المحاولة 3 مرات في حال الفشل
            endpointConfigurator.UseMessageRetry(r => r.Interval(3, 1000));
        }
    }

}
