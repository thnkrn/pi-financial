using System.Text.Json.Serialization;

namespace Pi.SetMarketData.Domain.Models.Response;

public class StatusResponse
{
    [JsonPropertyName("marketStatus")]
    public string? MarketStatus { get; set; }
}

public class MarketStatusResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("response")]
    public StatusResponse? Response { get; set; }
}