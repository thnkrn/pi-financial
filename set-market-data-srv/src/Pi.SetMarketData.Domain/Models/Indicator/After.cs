using Newtonsoft.Json;

namespace Pi.SetMarketData.Domain.Models.Indicator;

// TODO: Remove unused properties

public partial class After
{
    [JsonProperty("bucket")]
    public DateTimeOffset Bucket { get; set; }

    [JsonProperty("symbol")]
    public string? Symbol { get; set; }

    [JsonProperty("venue")]
    public string? Venue { get; set; }

    // [JsonProperty("open")]
    // public double Open { get; set; }

    // [JsonProperty("high")]
    // public double High { get; set; }

    // [JsonProperty("low")]
    // public long Low { get; set; }

    // [JsonProperty("close")]
    // public double Close { get; set; }

    // [JsonProperty("volume")]
    // public Volume? Volume { get; set; }

    // [JsonProperty("amount")]
    // public long Amount { get; set; }
}

// public class Volume
// {
//     [JsonProperty("scale")]
//     public int Scale { get; set; }

//     [JsonProperty("value")]
//     public string? Value { get; set; }
// }