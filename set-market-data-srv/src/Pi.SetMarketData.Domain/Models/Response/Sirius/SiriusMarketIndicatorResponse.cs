using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Pi.SetMarketData.Domain.Models.Response.Sirius;

public class SiriusMarketIndicatorResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("response")]
    public MarketIndicator? Response { get; set; }

    public static SiriusMarketIndicatorResponse? FromJson(string json) =>
        JsonConvert.DeserializeObject<SiriusMarketIndicatorResponse>(json);
}

public class MarketIndicator
{
    [JsonPropertyName("meta")]
    public ResponseMeta? Meta { get; set; }

    [JsonPropertyName("venue")]
    public string? Venue { get; set; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("candleType")]
    public string? CandleType { get; set; }

    [JsonPropertyName("candles")]
    public List<List<object>> Candles { get; set; } = [];

    [JsonPropertyName("indicators")]
    public Indicators Indicators { get; set; } = new Indicators();

    [JsonPropertyName("firstCandleTime")]
    public long FirstCandleTime { get; set; }
}

public class ResponseMeta
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

public class Indicators
{
    [JsonPropertyName("Boll")]
    public List<object?> Boll { get; set; } = [];

    [JsonPropertyName("Ema")]
    public List<object?> Ema { get; set; } = [];

    [JsonPropertyName("Kdj")]
    public List<object?> Kdj { get; set; } = [];

    [JsonPropertyName("Ma")]
    public List<object?> Ma { get; set; } = [];

    [JsonPropertyName("Macd")]
    public List<object?> Macd { get; set; } = [];

    [JsonPropertyName("Rsi")]
    public List<object?> Rsi { get; set; } = [];
}
