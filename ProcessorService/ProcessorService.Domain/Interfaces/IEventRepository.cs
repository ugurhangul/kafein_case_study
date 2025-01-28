using ProcessorService.Domain.Models;

namespace ProcessorService.Domain.Interfaces;

public interface IEventRepository
{
    Task<bool> SaveEventAsync(AuditEvent auditEvent, string indexName);
}