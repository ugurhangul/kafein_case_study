using ProcessorService.Application.Services;
using ProcessorService.Domain.Interfaces;
using ProcessorService.Domain.Models;
using ProcessorService.Infrastructure.Repositories;
using Nest;

namespace ProcessorService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

          

            // Register Elasticsearch client as a singleton for application-wide reuse
            builder.Services.AddScoped<IElasticClient>(_ =>
            {
                // Define connection settings for Elasticsearch, with default index set to "audit-events"
                var settings = new ConnectionSettings(new Uri("http://elasticsearch:9200"))
                    .DefaultIndex("audit-events");
                return new ElasticClient(settings);
            });

            // Register the event repository (implementation backed by Elasticsearch)
            builder.Services.AddScoped<IEventRepository, ElasticsearchEventRepository>();

            // Register the event processor service with scoped lifetime (created per request)
            builder.Services.AddScoped<IEventProcessor, EventProcessorService>();
            builder.Services.AddScoped<RuleConfigService>();
            // Register support for controllers in the application
            builder.Services.AddControllers();
            builder.Services.AddHostedService<Worker>();
            var app = builder.Build(); // Build the application pipeline

            // Map a POST endpoint for processing audit events
            app.MapPost("/process", async (AuditEvent auditEvent, IEventProcessor processor) =>
            {
                // Process the incoming audit event asynchronously using the processor service
                await processor.ProcessEventAsync(auditEvent);

                // Return a success result to the client
                return Results.Ok("Event processed successfully.");
            });

            // Run the application
            app.Run();
        }
    }
}