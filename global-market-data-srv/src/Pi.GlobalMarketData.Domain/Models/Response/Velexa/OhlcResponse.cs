using Newtonsoft.Json;

namespace Pi.GlobalMarketData.Domain.Models.Response.Velexa;

// ReSharper disable InvalidXmlDocComment
/// <summary>
///     [
///     {
///     "close": "23.23",
///     "high": "23.29",
///     "low": "23.21",
///     "open": "23.28",
///     "timestamp": 1462203000000,
///     "volume": "8894320"
///     }
///     ]
/// </summary>
public class VelexaOhlcResponse
{
    [JsonProperty("close")] public string Close { get; set; }

    [JsonProperty("high")] public string High { get; set; }

    [JsonProperty("low")] public string Low { get; set; }

    [JsonProperty("open")] public string Open { get; set; }

    [JsonProperty("timestamp")] public long Timestamp { get; set; }

    [JsonProperty("volume")] public string Volume { get; set; }
}