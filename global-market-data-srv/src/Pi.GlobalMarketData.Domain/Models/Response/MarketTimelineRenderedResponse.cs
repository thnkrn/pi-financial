using System.Text.Json.Serialization;

namespace Pi.GlobalMarketData.Domain.Models.Response;

public class Intermission
{
    [JsonPropertyName("from")]
    public int From { get; set; }

    [JsonPropertyName("to")]
    public int To { get; set; }
}

public class TimelineMeta
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

public class TimelineRenderedResponse
{
    [JsonPropertyName("venue")]
    public string? Venue { get; set; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("data")]
    public List<object[]>? Data { get; set; }

    [JsonPropertyName("intermissions")]
    public List<Intermission>? Intermissions { get; set; }

    [JsonPropertyName("meta")]
    public TimelineMeta? Meta { get; set; }
}

public class MarketTimelineRenderedResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("response")]
    public TimelineRenderedResponse? Response { get; set; }
}
