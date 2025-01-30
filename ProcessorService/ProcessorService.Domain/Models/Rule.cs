using System.Text.Json.Serialization;

namespace ProcessorService.Domain.Models;

public class Rule
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("eventType")]
    public string EventType { get; set; }

    [JsonPropertyName("isCritical")]
    public bool IsCritical { get; set; }
}