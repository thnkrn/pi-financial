namespace Pi.SetMarketData.DataMigrationJobProducer.Entities;

public class MigrationJob
{
    public required string Symbol { get; set; }
    public required string Venue { get; set; }
    public DateTime DateTimeFrom { get; set; }
    public DateTime DateTimeTo { get; set; }
}