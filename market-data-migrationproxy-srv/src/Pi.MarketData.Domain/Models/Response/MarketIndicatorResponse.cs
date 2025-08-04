using System.Text.Json.Serialization;

namespace Pi.MarketData.Domain.Models.Response;

public class Meta
{
    [JsonPropertyName("exchange")]
    public string? Exchange { get; set; }

    [JsonPropertyName("exchangeTimezone")]
    public string? ExchangeTimezone { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("offsetSeconds")]
    public int OffsetSeconds { get; set; }
}

public class IndicatorResponse
{
    [JsonPropertyName("meta")]
    public Meta? Meta { get; set; }

    [JsonPropertyName("venue")]
    public string? Venue { get; set; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("candleType")]
    public string? CandleType { get; set; }

    [JsonPropertyName("candles")]
    public List<List<object>>? Candles { get; set; }

    [JsonPropertyName("indicators")]
    public TechnicalIndicatorsResponse? Indicators { get; set; }

    [JsonPropertyName("firstCandleTime")]
    public int FirstCandleTime { get; set; }
}

public class MarketIndicatorResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("response")]
    public IndicatorResponse? Response { get; set; }

    [JsonPropertyName("debugStack")]
    public string? DebugStack { get; set; }
}

public class TechnicalIndicatorsResponse
{
    [JsonPropertyName("boll")]
    public List<object> Boll { get; set; } = [];

    [JsonPropertyName("ema")]
    public List<object> Ema { get; set; } = [];

    [JsonPropertyName("kdj")]
    public List<object> Kdj { get; set; } = [];

    [JsonPropertyName("ma")]
    public List<object> Ma { get; set; } = [];

    [JsonPropertyName("macd")]
    public List<object> Macd { get; set; } = [];

    [JsonPropertyName("rsi")]
    public List<object> Rsi { get; set; } = [];
}
