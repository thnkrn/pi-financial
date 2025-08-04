using System.Text.Json.Serialization;

namespace Pi.SetMarketData.Domain.Models.Request.Sirius;

public class SiriusMarketIndicatorRequest
{
    [JsonPropertyName("candleType")]
    public string? CandleType { get; set; }

    [JsonPropertyName("completeTradingDay")]
    public bool CompleteTradingDay { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("fromTimestamp")]
    public long FromTimestamp { get; set; }

    [JsonPropertyName("indicators")]
    public RequestIndicators? Indicators { get; set; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("toTimestamp")]
    public long ToTimestamp { get; set; }

    [JsonPropertyName("venue")]
    public string? Venue { get; set; }
}

// This data is fixed and should not be changed
public class RequestIndicators
{
    [JsonPropertyName("boll")]
    public string Boll { get; set; } = "20,2";

    [JsonPropertyName("ema")]
    public string Ema { get; set; } = "10,25";

    [JsonPropertyName("kdj")]
    public string Kdj { get; set; } = "14,3,3";

    [JsonPropertyName("ma")]
    public string Ma { get; set; } = "10,25";

    [JsonPropertyName("macd")]
    public string Macd { get; set; } = "12,26,9";

    [JsonPropertyName("rsi")]
    public string Rsi { get; set; } = "14";
}
