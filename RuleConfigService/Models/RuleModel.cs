namespace RuleConfigService.Models
{
    public class Rule
    {
        public int Id { get; set; }
        public string? EventType { get; set; }
        public bool IsCritical { get; set; }
    }
}