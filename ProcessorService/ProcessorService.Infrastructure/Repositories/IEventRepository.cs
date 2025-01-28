using Elasticsearch.Net;
using Nest;
using ProcessorService.Domain.Interfaces;
using AuditEvent = ProcessorService.Domain.Models.AuditEvent;

namespace ProcessorService.Infrastructure.Repositories;

public class ElasticsearchEventRepository(IElasticClient elasticClient) : IEventRepository
{
    public async Task<bool> SaveEventAsync(AuditEvent auditEvent, string indexName)
    {
        await EnsureIndexExistsAsync(indexName);
        var response = await elasticClient.IndexAsync(auditEvent, i => i.Index(indexName));
        return response.IsValid;
    }
    
    public async Task<bool> EnsureIndexExistsAsync(string indexName)
    {
        var existsResponse = await elasticClient.Indices.ExistsAsync(indexName);
        if (existsResponse.Exists) return true;
        var createResponse = await elasticClient.Indices.CreateAsync(indexName, c => c
            .Map<AuditEvent>(m => m
                .AutoMap()
            )
        );

        return createResponse.IsValid;
    }
}