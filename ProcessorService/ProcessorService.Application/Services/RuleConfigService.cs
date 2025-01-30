using System.Text.Json;
using ProcessorService.Domain.Models;

namespace ProcessorService.Application.Services;

public class RuleConfigService
{
    public async Task<List<Rule>> FetchRules()
    {
        var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync("http://rule-config-service:5001/api/rules");
            response.EnsureSuccessStatusCode();
            Console.WriteLine($"Response Status Code: {response.StatusCode}");
            var responseBody = await response.Content.ReadAsStringAsync();
            var rules = JsonSerializer.Deserialize<List<Rule>>(responseBody);
            if (rules == null || rules.Count == 0) // Rules not found in the config service
            {
                rules =
                [
                    new Rule { EventType = "SELECT", IsCritical = false }, // Non-critical SELECT events
                    new Rule { EventType = "UPDATE", IsCritical = true }, // Critical UPDATE events
                    new Rule { EventType = "DELETE", IsCritical = true }
                ];
            }

            Console.WriteLine($"Rules: {JsonSerializer.Serialize(rules)}");
            return rules;
        }
        catch (HttpRequestException ex)
        {
            // Log exception or handle it as needed
            Console.WriteLine($"HttpRequest failed: {ex.Message}");
        }
        finally
        {
            httpClient.Dispose();
        }

        Console.WriteLine("Rules not found in the config service");
        return [];
    }
}