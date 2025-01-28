namespace ProcessorService.Domain.Models;

public class Rule
{
    public string EventType { get; set; }
    public bool IsCritical { get; set; }
}