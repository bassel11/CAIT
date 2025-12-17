using MassTransit;

namespace Audit.Infrastructure.Consumers
{
    public class AuditLogCreatedConsumerDefinition : ConsumerDefinition<AuditLogCreatedConsumer>
    {
        public AuditLogCreatedConsumerDefinition()
        {
            // 👇👇 هنا تضع الاسم الذي تريده ويظهر في RabbitMQ
            EndpointName = "audit-service-main-queue";
        }
        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<AuditLogCreatedConsumer> consumerConfigurator)
        {
            // هام جداً: معالجة رسالة واحدة تلو الأخرى لضمان صحة سلسلة الـ Hash
            consumerConfigurator.UseConcurrentMessageLimit(1);

            // إعادة المحاولة في حال فشل الاتصال بقاعدة البيانات
            endpointConfigurator.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(2)));
        }
    }
}
