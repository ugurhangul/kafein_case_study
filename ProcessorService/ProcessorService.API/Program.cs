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
            builder.Services.AddSingleton<IElasticClient>(_ =>
            {
                // Define connection settings for Elasticsearch, with default index set to "audit-events"
                var settings = new ConnectionSettings(new Uri("http://elasticsearch:9200"))
                    .DefaultIndex("audit-events");
                return new ElasticClient(settings);
            });

            // Register the event repository (implementation backed by Elasticsearch)
            builder.Services.AddSingleton<IEventRepository, ElasticsearchEventRepository>();

            // Register the event processor service with scoped lifetime (created per request)
            builder.Services.AddScoped<IEventProcessor, EventProcessorService>(sp =>
            {
                // Retrieve event repository instance from DI container
                var repository = sp.GetRequiredService<IEventRepository>();

                // Define a set of rules for processing audit events
                var rules = new List<Rule>
                {
                    new() { EventType = "SELECT", IsCritical = false }, // Non-critical SELECT events
                    new() { EventType = "UPDATE", IsCritical = true }, // Critical UPDATE events
                    new() { EventType = "DELETE", IsCritical = true } // Critical DELETE events
                };

                // Return a new instance of EventProcessorService with the repository and rules
                return new EventProcessorService(repository, rules);
            });

            // Register support for controllers in the application
            builder.Services.AddControllers();

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