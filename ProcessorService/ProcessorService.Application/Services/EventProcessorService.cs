using ProcessorService.Domain.Interfaces;
using ProcessorService.Domain.Models;

namespace ProcessorService.Application.Services
{
    
    public class EventProcessorService(IEventRepository eventRepository, RuleConfigService ruleConfigService) : IEventProcessor
    {
        public async Task ProcessEventAsync(AuditEvent auditEvent)
        {
            var rules = await ruleConfigService.FetchRules();
            // Evaluate rules
            var matchingRule = rules.FirstOrDefault(rule => rule.EventType == auditEvent.EventType);
            Console.WriteLine($"Matching Rule: {matchingRule?.EventType}");
            auditEvent.IsCritical = matchingRule is { IsCritical: true };

            // Determine Index Name Dynamically
            var indexName = auditEvent.IsCritical ? "critical-events" : "audit-events";

            Console.WriteLine($"Saving event to {indexName} index. auditEvent: {auditEvent.IsCritical}");
            
            // Save to Elasticsearch
            var success = await eventRepository.SaveEventAsync(auditEvent, indexName);
            if (!success)
            {
                Console.WriteLine($"Failed to index document: {auditEvent.EventId}");
            }
        }

       
    }
}