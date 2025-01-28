using ProcessorService.Application.Services;
using ProcessorService.Domain.Interfaces;
using ProcessorService.Domain.Models;
using ProcessorService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace ProcessorService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register services
            builder.Services.AddSingleton<IElasticClient>(sp =>
            {
                var settings = new ConnectionSettings(new Uri("http://elasticsearch:9200"))
                    .DefaultIndex("audit-events");
                return new ElasticClient(settings);
            });

            builder.Services.AddSingleton<IEventRepository, ElasticsearchEventRepository>();
            builder.Services.AddScoped<IEventProcessor, EventProcessorService>(sp =>
            {
                var repository = sp.GetRequiredService<IEventRepository>();
                var rules = new List<Rule>
                {
                    new() { EventType = "SELECT", IsCritical = false },
                    new() { EventType = "UPDATE", IsCritical = true },
                    new() { EventType = "DELETE", IsCritical = true }
                };
                return new EventProcessorService(repository, rules);
            });

            builder.Services.AddControllers();

            var app = builder.Build();

            app.MapPost("/process", async (AuditEvent auditEvent, IEventProcessor processor) =>
            {
                await processor.ProcessEventAsync(auditEvent);
                return Results.Ok("Event processed successfully.");
            });

            app.Run();
        }
    }
}