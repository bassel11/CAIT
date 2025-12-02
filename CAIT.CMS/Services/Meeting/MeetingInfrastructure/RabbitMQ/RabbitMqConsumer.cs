using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MeetingInfrastructure.RabbitMQ
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly ILogger<RabbitMqConsumer> _logger;
        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("🚀 RabbitMqConsumer is starting...");

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("cms.meetings", ExchangeType.Topic, durable: true);

            _channel.QueueDeclare(
                queue: "cms.debug",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueBind(
                queue: "cms.debug",
                exchange: "cms.meetings",
                routingKey: "test.*");

            _logger.LogInformation("✔ RabbitMQ Consumer connected and queue bound successfully.");

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
            {
                _logger.LogError("❌ RabbitMQ channel is not initialized");
                return Task.CompletedTask;
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (sender, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation($"📥 Received from RabbitMQ: {json}");
                await Task.Yield();
            };

            _channel.BasicConsume(queue: "cms.debug", autoAck: true, consumer);

            _logger.LogInformation("⏳ RabbitMQ consumer started listening on queue 'cms.debug'.");

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
