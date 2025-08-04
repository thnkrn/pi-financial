using System.Text.Json.Serialization;

namespace Pi.SetMarketData.Application.Queries;

public class HomeInstrumentParameter
{
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }
    [JsonPropertyName("venue")]
    public string? Venue { get; set; }
}

public class HomeInstrumentsProxyRequest
{
    [JsonPropertyName("param")]
    public List<HomeInstrumentParameter>? Param { get; set; }
}