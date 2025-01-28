using System.Text.Json.Serialization;

namespace ReportingAlertingService.Models;

public class Rule
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("eventType")]
    public string EventType { get; set; }

    [JsonPropertyName("isCritical")]
    public bool IsCritical { get; set; }
}