using System.Text.Json.Serialization;

namespace Pi.SetMarketData.Domain.Models.Response.MarketDataManagement;

public class CuratedFilterResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("filterId")]
    public int FilterId { get; set; }

    [JsonPropertyName("filterName")]
    public string? FilterName { get; set; }

    [JsonPropertyName("filterCategory")]
    public string? FilterCategory { get; set; }

    [JsonPropertyName("filterType")]
    public string? FilterType { get; set; }

    [JsonPropertyName("categoryPriority")]
    public int CategoryPriority { get; set; }

    [JsonPropertyName("groupName")]
    public string? GroupName { get; set; }

    [JsonPropertyName("subGroupName")]
    public string? SubGroupName { get; set; }

    [JsonPropertyName("curatedListId")]
    public int CuratedListId { get; set; }

    [JsonPropertyName("listName")]
    public string? ListName { get; set; }

    [JsonPropertyName("listSource")]
    public string? ListSource { get; set; }

    [JsonPropertyName("isDefault")]
    public int? IsDefault { get; set; }

    [JsonPropertyName("highlight")]
    public int Highlight { get; set; }

    [JsonPropertyName("ordering")]
    public int Ordering { get; set; }
}