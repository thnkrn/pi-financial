using System.Text.Json.Serialization;

namespace Pi.BackofficeService.Application.Models.CuratedManager;

public record CuratedFilterItem
{
    [JsonConstructor]
    public CuratedFilterItem(
        string id,
        int filterId,
        string filterName,
        string filterCategory,
        string filterType,
        int categoryPriority,
        int curatedListId,
        string listName,
        string listSource,
        bool isDefault,
        bool highlight,
        int ordering)
    {
        Id = id;
        FilterId = filterId;
        FilterName = filterName ?? string.Empty;
        FilterCategory = filterCategory ?? string.Empty;
        FilterType = filterType ?? string.Empty;
        CategoryPriority = categoryPriority;
        CuratedListId = curatedListId;
        ListName = listName ?? string.Empty;
        ListSource = listSource ?? string.Empty;
        IsDefault = isDefault;
        Highlight = highlight;
        Ordering = ordering;
    }

    [JsonPropertyName("id")]
    public string Id { get; }

    [JsonPropertyName("filterId")]
    public int FilterId { get; }

    [JsonPropertyName("filterName")]
    public string FilterName { get; }

    [JsonPropertyName("filterCategory")]
    public string FilterCategory { get; }

    [JsonPropertyName("filterType")]
    public string FilterType { get; }

    [JsonPropertyName("categoryPriority")]
    public int CategoryPriority { get; }

    [JsonPropertyName("curatedListId")]
    public int CuratedListId { get; }

    [JsonPropertyName("listName")]
    public string ListName { get; }

    [JsonPropertyName("listSource")]
    public string ListSource { get; }

    [JsonPropertyName("isDefault")]
    public bool IsDefault { get; }

    [JsonPropertyName("highlight")]
    public bool Highlight { get; }

    [JsonPropertyName("ordering")]
    public int Ordering { get; }
}

public record CuratedFilterGroup
{
    [JsonConstructor]
    public CuratedFilterGroup(string name, List<CuratedFilterItem> data)
    {
        Name = name ?? string.Empty;
        this.Data = data ?? new List<CuratedFilterItem>();
    }

    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonPropertyName("data")]
    public List<CuratedFilterItem> Data { get; }
}