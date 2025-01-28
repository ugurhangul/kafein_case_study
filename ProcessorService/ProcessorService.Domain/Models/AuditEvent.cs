﻿namespace ProcessorService.Domain.Models
{
    public class AuditEvent
    {
        public string EventId { get; set; } = Guid.NewGuid().ToString();
        public string EventType { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Username { get; set; }
        public string DatabaseName { get; set; }
        public string Statement { get; set; }
        public string Severity { get; set; }
        public bool IsCritical { get; set; }
    }

   
}