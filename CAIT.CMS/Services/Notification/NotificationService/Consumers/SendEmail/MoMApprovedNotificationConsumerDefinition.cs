using MassTransit;

namespace NotificationService.Consumers.SendEmail
{
    public class MoMApprovedNotificationConsumerDefinition : ConsumerDefinition<MoMApprovedNotificationConsumer>
    {
        public MoMApprovedNotificationConsumerDefinition()
        {
            // تحديد اسم الطابور يدوياً ليكون واضحاً في RabbitMQ
            EndpointName = "notification-send-email";
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<MoMApprovedNotificationConsumer> consumerConfigurator)
        {
            // ✅ سياسة إعادة المحاولة (Retry Policy)
            // في حال فشل الإرسال (مثلاً سيرفر الإيميل مشغول)، سيحاول 3 مرات بفاصل 5 ثوانٍ
            endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

            // يمكن إضافة Circuit Breaker لحماية النظام إذا كان السيرفر معطلاً تماماً
            // endpointConfigurator.UseCircuitBreaker(cb => ...);
        }
    }
}
