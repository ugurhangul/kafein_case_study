using ProcessorService.Domain.Interfaces;
using ProcessorService.Domain.Models;

namespace ProcessorService.Application.Services
{
    
    public class EventProcessorService(IEventRepository eventRepository, List<Rule> rules) : IEventProcessor
    {
        public async Task ProcessEventAsync(AuditEvent auditEvent)
        {
            // Evaluate rules
            var matchingRule = rules.FirstOrDefault(rule => rule.EventType == auditEvent.EventType);
            auditEvent.IsCritical = matchingRule != null && matchingRule.IsCritical;

            // Determine Index Name Dynamically
            var indexName = auditEvent.IsCritical ? "critical-events" : "non-critical-events";

            // Save to Elasticsearch
            var success = await eventRepository.SaveEventAsync(auditEvent, indexName);
            if (!success)
            {
                Console.WriteLine($"Failed to index document: {auditEvent.EventId}");
            }
        }
    }
}