using Newtonsoft.Json;

namespace Pi.GlobalMarketData.Domain.Models.DataMigration;

public class MigrationJob
{
    public string? Symbol { get; set; }
    public string? Venue { get; set; }
    public DateTime DateTimeFrom { get; set; }
    public DateTime DateTimeTo { get; set; }

    public static MigrationJob? FromJson(string json)
    {
        return JsonConvert.DeserializeObject<MigrationJob>(json);
    }
}