using MeetingApplication.Integrations;
using MeetingInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MeetingInfrastructure.Outbox
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<OutboxProcessor> _logger;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public OutboxProcessor(IServiceProvider sp, ILogger<OutboxProcessor> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<MeetingDbContext>();
                    var router = scope.ServiceProvider.GetRequiredService<IOutboxRouter>();

                    var messages = await db.OutboxMessages
                        .Where(x => !x.Processed && x.Attempts < 5)
                        .OrderBy(x => x.OccurredAt)
                        .Take(20)
                        .ToListAsync(stoppingToken);

                    foreach (var msg in messages)
                    {
                        try
                        {
                            var handler = router.Resolve(msg.Type);
                            await handler.HandleAsync(msg, stoppingToken);

                            msg.Processed = true;
                            msg.ProcessedAt = DateTime.UtcNow;
                        }
                        catch (Exception ex)
                        {
                            msg.Attempts++;
                            msg.LastError = ex.Message;
                            _logger.LogError(ex, "Failed processing outbox message {Id} type {Type}", msg.Id, msg.Type);
                        }
                        db.OutboxMessages.Update(msg);
                        await db.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OutboxProcessor loop failed");
                }

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }
    }
}
