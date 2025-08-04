using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pi.MarketData.Domain.Models.Response;

public class GlobalInstrumentInfoResponse
{
    [JsonPropertyName("minimalPriceIncrement")]
    public string? MinimalPriceIncrement { get; set; }

    [JsonPropertyName("minimalQuantityIncrement")]
    public string? MinimalQuantityIncrement { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}

public class GlobalMarketInstrumentInfoResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("response")]
    public GlobalInstrumentInfoResponse? Response { get; set; }
}
