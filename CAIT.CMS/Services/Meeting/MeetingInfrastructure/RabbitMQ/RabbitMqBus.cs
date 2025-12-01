using MeetingApplication.Integrations;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MeetingInfrastructure.RabbitMQ
{
    public class RabbitMqBus : IMessageBus, IDisposable
    {
        private readonly IConnection _connection;
        private readonly JsonSerializerOptions _jsonOptions =
            new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public RabbitMqBus(IConfiguration config)
        {
            var factory = new ConnectionFactory()
            {
                HostName = config["RabbitMQ:Host"] ?? "localhost",
                UserName = config["RabbitMQ:User"] ?? "guest",
                Password = config["RabbitMQ:Pass"] ?? "guest"
            };

            _connection = factory.CreateConnection();
        }

        public Task PublishAsync(string routingKey, object payload, CancellationToken ct = default)
        {
            using var channel = _connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: "cms.meetings",
                type: ExchangeType.Topic,
                durable: true
            );

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload, _jsonOptions));

            var props = channel.CreateBasicProperties();
            props.Persistent = true;

            channel.BasicPublish(
                exchange: "cms.meetings",
                routingKey: routingKey,
                basicProperties: props,
                body: body
            );

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
