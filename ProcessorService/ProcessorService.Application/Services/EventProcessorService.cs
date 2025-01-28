using ProcessorService.Domain;
using ProcessorService.Domain.Interfaces;
using ProcessorService.Domain.Models;

namespace ProcessorService.Application.Services
{
    // EventProcessorService.cs (Application katmanında yer almalı)
    public class EventProcessorService(IEventRepository eventRepository, List<Rule> rules) : IEventProcessor
    {
        public async Task ProcessEventAsync(AuditEvent auditEvent)
        {
            // Kuralları değerlendir
            var matchingRule = rules.FirstOrDefault(rule => rule.EventType == auditEvent.EventType);
            auditEvent.IsCritical = matchingRule != null && matchingRule.IsCritical;

            // Index Name'i Dinamik Belirleme
            var indexName = auditEvent.IsCritical ? "critical-events" : "non-critical-events";

            // Elasticsearch'e Kaydet
            var success = await eventRepository.SaveEventAsync(auditEvent, indexName);
            if (!success)
            {
                Console.WriteLine($"Failed to index document: {auditEvent.EventId}");
            }
        }
    }


  
}