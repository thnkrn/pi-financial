using System.Text.Json.Serialization;

namespace Pi.SetMarketDataWSS.Domain.Models.Response;

public class Body
{
    [JsonPropertyName("filterCategory")] public string? FilterCategory { get; set; }

    [JsonPropertyName("filterType")] public string? FilterType { get; set; }

    [JsonPropertyName("supportSecondaryFilter")]
    public bool SupportSecondaryFilter { get; set; }

    [JsonPropertyName("filters")] public List<Filter>? Filters { get; set; }

    [JsonPropertyName("order")] public int Order { get; set; }
}

public class Filter
{
    [JsonPropertyName("filterId")] public int FilterId { get; set; }

    [JsonPropertyName("filterName")] public string? FilterName { get; set; }

    [JsonPropertyName("isHighLight")] public bool IsHighLight { get; set; }

    [JsonPropertyName("isDefault")] public bool IsDefault { get; set; }

    [JsonPropertyName("order")] public int Order { get; set; }
}

public class FiltersResponse
{
    [JsonPropertyName("data")] public List<Body>? Data { get; set; }
}

public class MarketFiltersResponse
{
    [JsonPropertyName("code")] public string? Code { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }

    [JsonPropertyName("response")] public FiltersResponse? Response { get; set; }

    [JsonPropertyName("debugStack")] public string? DebugStack { get; set; }
}