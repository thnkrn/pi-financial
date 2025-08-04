using Newtonsoft.Json;

namespace Pi.SetMarketData.Domain.Models.DataMigration;

public class MigrationJob
{
    public string? Symbol { get; set; }
    public string? Venue { get; set; }
    public DateTime DateTimeFrom { get; set; }
    public DateTime DateTimeTo { get; set; }

    public static MigrationJob? FromJson(string json) =>
        JsonConvert.DeserializeObject<MigrationJob>(json);
}
