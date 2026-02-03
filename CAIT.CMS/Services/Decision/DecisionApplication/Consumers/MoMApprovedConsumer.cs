using BuildingBlocks.Contracts.Meeting.MoMs.IntegrationEvents;
using MassTransit;

namespace DecisionApplication.Consumers
{
    public class MoMApprovedConsumer : IConsumer<MoMApprovedIntegrationEvent>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILogger<MoMApprovedConsumer> _logger;

        public MoMApprovedConsumer(
            IApplicationDbContext dbContext,
            ILogger<MoMApprovedConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MoMApprovedIntegrationEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("📥 Received MoM Approved Event for Meeting {MeetingId}", message.MeetingId);

            // تحويل المعرفات إلى Value Objects للاستخدام
            var momId = MoMId.Of(message.MoMId);
            var meetingId = MeetingId.Of(message.MeetingId);

            // 1. Idempotency Check (التحقق من عدم التكرار)
            // استخدام الـ Value Object في الاستعلام
            var alreadyProcessed = await _dbContext.Decisions
                .AnyAsync(d => d.MoMId == momId, context.CancellationToken);

            if (alreadyProcessed)
            {
                _logger.LogWarning("⚠️ Decisions for MoM {MoMId} already exist. Skipping.", message.MoMId);
                return;
            }

            // 2. معالجة القرارات
            if (!message.Decisions.Any())
            {
                _logger.LogInformation("ℹ️ No decisions found in this MoM.");
                return;
            }

            foreach (var decisionDto in message.Decisions)
            {
                // إنشاء القرار باستخدام المصنع المخصص
                var decision = Decision.CreateDecisionFromMoM(
                    id: DecisionId.Of(Guid.NewGuid()),
                    title: DecisionTitle.Of(decisionDto.Title),
                    // نسخ النص للغتين مؤقتاً لأن الحدث يحوي نصاً واحداً
                    text: DecisionText.Of(decisionDto.Content, decisionDto.Content),
                    meetingId: meetingId,
                    momId: momId
                );

                _dbContext.Decisions.Add(decision);
            }

            // 3. الحفظ
            await _dbContext.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation("✅ Successfully created {Count} official decisions from MoM.", message.Decisions.Count);
        }
    }
}
