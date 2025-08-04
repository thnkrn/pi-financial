using Newtonsoft.Json;

namespace Pi.MarketData.Domain.Models.Response.MarketDataManagement;

public class CuratedFilterResponse
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("filterId")]
    public int FilterId { get; set; }

    [JsonProperty("filterName")]
    public string? FilterName { get; set; }

    [JsonProperty("filterCategory")]
    public string? FilterCategory { get; set; }

    [JsonProperty("filterType")]
    public string? FilterType { get; set; }

    [JsonProperty("categoryPriority")]
    public int CategoryPriority { get; set; }

    [JsonProperty("groupName")]
    public string? GroupName { get; set; }

    [JsonProperty("subGroupName")]
    public string? SubGroupName { get; set; }

    [JsonProperty("curatedListId")]
    public int? CuratedListId { get; set; }

    [JsonProperty("listName")]
    public string? ListName { get; set; }

    [JsonProperty("listSource")]
    public string? ListSource { get; set; }

    [JsonProperty("isDefault")]
    public int? IsDefault { get; set; }

    [JsonProperty("highlight")]
    public int Highlight { get; set; }

    [JsonProperty("ordering")]
    public int Ordering { get; set; }
}