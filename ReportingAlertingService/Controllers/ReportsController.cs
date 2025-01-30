using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Nest;
using ReportingAlertingService.Models;

namespace ReportingAlertingService.Controllers;

// Controller class to handle API requests for reports
[ApiController]
[Route("api/[controller]")]
public class ReportsController(IElasticClient elasticClient, IHttpClientFactory httpClientFactory) : ControllerBase
{
    // Endpoint to fetch critical events from Elasticsearch between the provided startDate and endDate
    [HttpGet("critical-events")]
    public async Task<IActionResult> GetCriticalEvents(DateTime startDate, DateTime endDate)
    {
        // Perform a search query using Elasticsearch client to fetch records from the "critical-events" index
        // where IsCritical is true
        var allRecordsResponse = await elasticClient.SearchAsync<AuditEvent>(s => s
            .Index("critical-events") // Specify the Elasticsearch index name
            .Query(q => q.MatchAll() && q.Term(ev => ev.IsCritical, true)) // Filter to only critical events
        );

        // Check if the response from Elasticsearch is valid
        if (!allRecordsResponse.IsValid)
        {
            // Log the Elasticsearch error message to the console
            Console.WriteLine($"Elasticsearch error: {allRecordsResponse.ServerError.Error.Reason}");
            
            // Return a 500 error response indicating that fetching data failed
            return Problem("Failed to fetch critical events from Elasticsearch.");
        }

        // Return the documents fetched from Elasticsearch as a response in case of a valid response
        return Ok(allRecordsResponse.Documents);
    }

    // Endpoint to fetch critical events from Elasticsearch between the provided startDate and endDate
    [HttpGet("audit-events")]
    public async Task<IActionResult> GetOtherEvents(DateTime startDate, DateTime endDate)
    {
        // Perform a search query using Elasticsearch client to fetch records from the "audit-events" index
        // where IsCritical is not true
        var allRecordsResponse = await elasticClient.SearchAsync<AuditEvent>(s => s
                .Index("audit-events") // Specify the Elasticsearch index name
                .Query(q => q.MatchAll() && q.Term(ev => ev.IsCritical, false)) // Filter to only critical events
        );

        // Check if the response from Elasticsearch is valid
        if (!allRecordsResponse.IsValid)
        {
            // Log the Elasticsearch error message to the console
            Console.WriteLine($"Elasticsearch error: {allRecordsResponse.ServerError.Error.Reason}");
            
            // Return a 500 error response indicating that fetching data failed
            return Problem("Failed to fetch events from Elasticsearch.");
        }

        // Return the documents fetched from Elasticsearch as a response in case of a valid response
        return Ok(allRecordsResponse.Documents);
    }

    
    // Endpoint to fetch rules from a remote configuration service via HTTP
    [HttpGet("config/rules")]
    public async Task<IActionResult> GetRulesFromConfigService()
    {
        // Create a new HTTP client using the factory
        var client = httpClientFactory.CreateClient();
        
        // Call the "rule-config-service" API to fetch the rules
        var response = await client.GetAsync("http://rule-config-service:5001/api/rules");

        // Check if the HTTP response was unsuccessful
        if (!response.IsSuccessStatusCode)
        {
            // Return a 500 error response if the external service doesn't respond successfully
            return Problem("Failed to fetch rules from Config Service");
        }

        // Deserialize the response content into a collection of Rule objects
        var rules = JsonSerializer.Deserialize<List<Rule>>(await response.Content.ReadAsStringAsync());
        
        // Return the deserialized rules as the response
        return Ok(rules);
    }
}