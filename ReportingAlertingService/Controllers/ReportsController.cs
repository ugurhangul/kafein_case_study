using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Nest;
using ReportingAlertingService.Models;

namespace ReportingAlertingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController(IElasticClient elasticClient, IHttpClientFactory httpClientFactory) : ControllerBase
{
    [HttpGet("critical-events")]
    public async Task<IActionResult> GetCriticalEvents(DateTime startDate, DateTime endDate)
    {
        var allRecordsResponse = await elasticClient.SearchAsync<AuditEvent>(s => s
            .Index("critical-events")
            .Query(q => q.MatchAll() && q.Term(ev => ev.IsCritical, true))
        );

        if (!allRecordsResponse.IsValid)
        {
            Console.WriteLine($"Elasticsearch error: {allRecordsResponse.ServerError.Error.Reason}");
            return Problem("Failed to fetch critical events from Elasticsearch.");
        }

        return Ok(allRecordsResponse.Documents);
    }

    [HttpGet("config/rules")]
    public async Task<IActionResult> GetRulesFromConfigService()
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync("http://rule-config-service:5001/api/rules");

        if (!response.IsSuccessStatusCode)
        {
            return Problem("Failed to fetch rules from Config Service");
        }

        var rules = JsonSerializer.Deserialize<List<Rule>>(await response.Content.ReadAsStringAsync());
        return Ok(rules);
    }
}