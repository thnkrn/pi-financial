using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Request;

public class UserInstrumentFavouriteRequest
{
    [JsonPropertyName("type")] public string? Type { get; set; }

    [JsonPropertyName("value")] public string? Value { get; set; }
}