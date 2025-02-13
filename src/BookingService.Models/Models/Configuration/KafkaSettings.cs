namespace BookingService.Models.Models.Configuration;

public class KafkaSettings
{
    public required IEnumerable<string> BootstrapServers { get; set; }
    public required string SaslUserName { get; set; }
    public required string SaslPassword { get; set; }
    public required string SchemaRegistry { get; set; }
}
