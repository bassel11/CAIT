using BuildingBlocks.Contracts.Decision.IntegrationEvents;
using MassTransit;

namespace DecisionApplication.Consumers
{
    // المستهلك يستمع للحدث المحدد
    public class DecisionCreatedConsumer : IConsumer<DecisionCreatedIntegrationEvent>
    {
        private readonly ILogger<DecisionCreatedConsumer> _logger;

        public DecisionCreatedConsumer(ILogger<DecisionCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DecisionCreatedIntegrationEvent> context)
        {
            // 1. الوصول لبيانات الرسالة
            var message = context.Message;

            // 2. تسجيل اللوج (هذا ما سيظهر لك في الـ Console ويؤكد نجاح العملية)
            _logger.LogInformation(
                "✅ [Consumer] Received Decision Created Integration Event.\n" +
                "   -> DecisionId: {DecisionId}\n" +
                "   -> MeetingId: {MeetingId}\n" +
                "   -> Title: {Title}",
                message.DecisionId,
                message.MeetingId,
                message.Title);

            // 3. هنا تضع المنطق الخاص بك (مثلاً: إرسال إشعار، تحديث جدول آخر، إلخ)
            try
            {
                // محاكاة عملية معالجة
                await Task.Delay(100);

                _logger.LogInformation("🎉 Event Processed Successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing message for decision {Id}", message.DecisionId);
                // في حالة وجود خطأ، MassTransit سيقوم بإعادة المحاولة (Retry) تلقائياً
                throw;
            }
        }
    }
}
