using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IKafkaProducer>(_ =>
{
    // Configure Kafka Producer
    var kafkaConfig = new ProducerConfig { BootstrapServers = "kafka:9092" };
    return new KafkaProducer(kafkaConfig);
});
var app = builder.Build();

app.MapPost("/produce", async ([FromBody] AuditEvent auditEvent, [FromServices] IKafkaProducer producer) =>
{
    try
    {
        var result = await producer.ProduceAsync("audit-events", auditEvent);
        return Results.Ok($"Event sent to Kafka: {result}");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error sending message to Kafka: {ex.Message}");
    }
});

app.Run();

// Kafka Producer Abstraction for Testing
public interface IKafkaProducer
{
    Task<string> ProduceAsync(string topic, AuditEvent auditEvent);
}

// Kafka Producer Implementation
public class KafkaProducer(ProducerConfig config) : IKafkaProducer
{
    private readonly IProducer<string, string> _producer = new ProducerBuilder<string, string>(config).Build();

  
    public async Task<string> ProduceAsync(string topic, AuditEvent auditEvent)
    {
        var message = new Message<string, string>
        {
            Key = auditEvent.EventId,
            Value = JsonSerializer.Serialize(auditEvent)
        };

        var result = await _producer.ProduceAsync(topic, message);
        return result.TopicPartitionOffset.ToString();
    }
}

// AuditEvent Model
public class AuditEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public string EventType { get; set; } = "Unknown";
    public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
    public string Username { get; set; } = "system";
    public string DatabaseName { get; set; } = "default";
    public string Statement { get; set; } = "N/A";
    public string Severity { get; set; } = "Normal";
}