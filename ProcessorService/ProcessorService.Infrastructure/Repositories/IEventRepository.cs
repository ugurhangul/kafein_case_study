using Nest;
using ProcessorService.Domain.Interfaces;
using AuditEvent = ProcessorService.Domain.Models.AuditEvent;

namespace ProcessorService.Infrastructure.Repositories;

// The ElasticsearchEventRepository class is responsible for handling CRUD operations on Elasticsearch
// for storing and querying audit events. It implements IEventRepository, making it conform to a 
// common repository pattern.
public class ElasticsearchEventRepository(IElasticClient elasticClient) : IEventRepository
{
    // Saves the given audit event to the specified Elasticsearch index.
    // Ensures that the index exists before attempting to write the event.
    // Returns true if the operation is successful, otherwise false.
    public async Task<bool> SaveEventAsync(AuditEvent auditEvent, string indexName)
    {
        // Ensure the index exists in Elasticsearch, create it if necessary
        await EnsureIndexExistsAsync(indexName);

        // Indexes the audit event into the specified index
        var response = await elasticClient.IndexAsync(auditEvent, i => i.Index(indexName));

        // Return whether the indexing operation was successful
        return response.IsValid;
    }
    
    // Ensures that the specified index exists in Elasticsearch.
    // If the index doesn't exist, create it using the map for AuditEvent.
    // Returns true if the index exists or was successfully created.
    public async Task<bool> EnsureIndexExistsAsync(string indexName)
    {
        // Check if the specified index already exists
        var existsResponse = await elasticClient.Indices.ExistsAsync(indexName);
        if (existsResponse.Exists) return true; // Return early if the index already exists

        // Create the index with mappings for AuditEvent if it doesn't exist
        var createResponse = await elasticClient.Indices.CreateAsync(indexName, c => c
            .Map<AuditEvent>(m => m
                .AutoMap() // Automatically map properties of the AuditEvent model
            )
        );

        // Return whether the index creation operation was successful
        return createResponse.IsValid;
    }
}