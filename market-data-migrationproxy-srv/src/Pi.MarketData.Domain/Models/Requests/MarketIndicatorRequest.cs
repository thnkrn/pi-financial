using System.Text.Json.Serialization;

namespace Pi.MarketData.Domain.Models.Requests;

public class Indicators
{
    [JsonPropertyName("boll")]
    public string? Boll { get; set; }

    [JsonPropertyName("ema")]
    public string? Ema { get; set; }

    [JsonPropertyName("kdj")]
    public string? Kdj { get; set; }

    [JsonPropertyName("ma")]
    public string? Ma { get; set; }

    [JsonPropertyName("macd")]
    public string? Macd { get; set; }

    [JsonPropertyName("rsi")]
    public string? Rsi { get; set; }
}

public class MarketIndicatorRequest : CommonPayload
{
    [JsonPropertyName("candleType")]
    public string? CandleType { get; set; }

    [JsonPropertyName("completeTradingDay")]
    public bool CompleteTradingDay { get; set; }

    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    [JsonPropertyName("fromTimestamp")]
    public int FromTimestamp { get; set; }

    [JsonPropertyName("indicators")]
    public Indicators? Indicators { get; set; }

    [JsonPropertyName("toTimestamp")]
    public int ToTimestamp { get; set; }

}
