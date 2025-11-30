using MeetingInfrastructure.Data;
using MeetingInfrastructure.Integrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MeetingInfrastructure.Outbox
{
    public class OutboxProcessorHostedService : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<OutboxProcessorHostedService> _logger;


        public OutboxProcessorHostedService(IServiceProvider sp, ILogger<OutboxProcessorHostedService> logger)
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
                    var publisher = scope.ServiceProvider.GetRequiredService<BusPublisherStub>();
                    var outlook = scope.ServiceProvider.GetRequiredService<OutlookClientStub>();


                    var pending = db.OutboxMessages.Where(o => !o.Processed).OrderBy(o => o.OccurredAt).Take(20).ToList();


                    foreach (var msg in pending)
                    {
                        try
                        {
                            // Simple dispatch by type
                            if (msg.Type == "MoMApproved")
                            {
                                var payload = JsonSerializer.Deserialize<JsonElement>(msg.Payload);
                                // publish to bus
                                await publisher.PublishAsync(msg.Payload);
                                // call outlook service
                                await outlook.AttachFileToMeetingAsync("meetingId", "storagePath");
                            }


                            msg.Processed = true;
                            msg.ProcessedAt = DateTime.UtcNow;
                            db.OutboxMessages.Update(msg);
                            await db.SaveChangesAsync(stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            msg.Attempts += 1;
                            db.OutboxMessages.Update(msg);
                            await db.SaveChangesAsync(stoppingToken);
                            _logger.LogError(ex, "Failed to process outbox message {Id}", msg.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Outbox processor error");
                }


                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

    }

}
