using System.Text.Json;
using Confluent.Kafka;
using Nest;
using ProcessorService.Application.Services;
using ProcessorService.Domain.Interfaces;
using ProcessorService.Domain.Models;

namespace ProcessorService.API
;

public class Worker : BackgroundService
{
    private readonly IEventProcessor _eventProcessorService;
    private readonly ConsumerConfig _consumerConfig;
    private readonly IElasticClient _elasticClient;

    public Worker(IEventProcessor eventProcessorService)
    {
        _eventProcessorService = eventProcessorService;
        // Kafka Consumer Configuration
        _consumerConfig = new ConsumerConfig
        {
            GroupId = "processor-group",
            BootstrapServers = "kafka:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        // Elasticsearch Client Configuration
        var settings = new ConnectionSettings(new Uri("http://elasticsearch:9200"))
            .DefaultIndex("audit-events");
        _elasticClient = new ElasticClient(settings);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
        consumer.Subscribe("audit-events");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var auditEvent = JsonSerializer.Deserialize<AuditEvent>(consumeResult.Message.Value);

                // Elasticsearch'e kaydetme
                var response = await _elasticClient.IndexDocumentAsync(auditEvent, stoppingToken);
                if (!response.IsValid)
                {
                    Console.WriteLine($"Error indexing document: {response.OriginalException}");
                }
                else
                {
                    Console.WriteLine($"Worker: {auditEvent.EventId}");
                    if (auditEvent != null) await _eventProcessorService.ProcessEventAsync(auditEvent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error consuming message: {ex.Message}");
            }
        }
    }
}
