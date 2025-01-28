using ProcessorService.Domain.Models;

namespace ProcessorService.Domain.Interfaces;

public interface IEventProcessor
{
    Task ProcessEventAsync(AuditEvent auditEvent);
}