using System.Text.Json.Serialization;

namespace Pi.MarketData.Domain.Models.Response;

public class ProfileDescriptionResponse
{
    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("websiteLink")] public string? WebsiteLink { get; set; }

    [JsonPropertyName("websiteLinkName")] public string? WebsiteLinkName { get; set; }

    [JsonPropertyName("amcCode")] public string? AmcCode { get; set; }

    [JsonPropertyName("amcLogo")] public string? AmcLogo { get; set; }

    [JsonPropertyName("amcFriendlyName")] public string? AmcFriendlyName { get; set; }
}

public class MarketProfileDescriptionResponse
{
    [JsonPropertyName("code")] public string? Code { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("response")] public ProfileDescriptionResponse? Response { get; set; }

    [JsonPropertyName("debugStack")] public string? DebugStack { get; set; }
}