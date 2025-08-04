using Newtonsoft.Json;

namespace Pi.SetMarketData.Domain.Models.Indicator;
public class IndicatorMigrationMessage
{
    public string Venue { get; set; }

    public string Symbol { get; set; }

    public DateTimeOffset DateTimeFrom { get; set; }
    public DateTimeOffset DateTimeTo { get; set; }

    public string Timeframe { get; set; }
}