using Newtonsoft.Json;

namespace Pi.MarketData.Search.API.Models;

public record MigrationProxyResponse<T>(T Response)
{
    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("response")]
    public T Response { get; set; } = Response;
}

public record HomeInstrumentResponse
{
    public required IEnumerable<HomeInstrumentItem> InstrumentList { get; init; } = new List<HomeInstrumentItem>();
}

public record HomeInstrumentItem
{
    public required int Order { get; init; }
    public required string InstrumentType { get; init; }
    public required string InstrumentCategory { get; init; }
    public required string Venue { get; init; }
    public required string Symbol { get; init; }
    public required string FriendlyName { get; init; }
    public required string Logo { get; init; }
    public required string Unit { get; init; }
    public required decimal Price { get; init; }
    public required decimal PriceChange { get; init; }
    public required decimal PriceChangeRatio { get; init; }
    public required decimal TotalValue { get; init; }
    public required decimal TotalVolume { get; init; }
}
