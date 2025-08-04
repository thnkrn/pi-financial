using System.Text.Json.Serialization;

namespace Pi.BackofficeService.Application.Models;

public enum CuratedListSource
{
    [JsonPropertyName("GE")]
    GE,
    [JsonPropertyName("SET")]
    SET
}

public enum CuratedType
{
    [JsonPropertyName("Logical")]
    Logical,
    [JsonPropertyName("Manual")]
    Manual
}

public record CuratedListItem
{
    [JsonConstructor]
    public CuratedListItem(
        string id,
        string idString,
        int curatedListId,
        string? curatedListCode,
        string curatedType,
        string relevantTo,
        string name,
        string hashtag,
        int? ordering,
        CuratedListSource? curatedListSource,
        DateTime? createTime,
        DateTime? updateTime,
        string updateBy)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        IdString = idString;
        CuratedListId = curatedListId;
        CuratedListCode = curatedListCode;
        CuratedType = curatedType ?? string.Empty;
        RelevantTo = relevantTo ?? string.Empty;
        Name = name ?? string.Empty;
        Hashtag = hashtag ?? string.Empty;
        Ordering = ordering;
        CuratedListSource = curatedListSource;
        CreateTime = createTime;
        UpdateTime = updateTime;
        UpdateBy = updateBy ?? string.Empty;
    }

    [JsonPropertyName("id")]
    public string Id { get; }

    [JsonPropertyName("idString")]
    public string IdString { get; }

    [JsonPropertyName("curatedListId")]
    public int CuratedListId { get; }

    [JsonPropertyName("curatedListCode")]
    public string? CuratedListCode { get; }

    [JsonPropertyName("curatedType")]
    public string CuratedType { get; }

    [JsonPropertyName("relevantTo")]
    public string RelevantTo { get; }

    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonPropertyName("hashtag")]
    public string Hashtag { get; }

    [JsonPropertyName("ordering")]
    public int? Ordering { get; }

    [JsonPropertyName("curatedListSource")]
    public CuratedListSource? CuratedListSource { get; }

    [JsonPropertyName("createTime")]
    public DateTime? CreateTime { get; }

    [JsonPropertyName("updateTime")]
    public DateTime? UpdateTime { get; }

    [JsonPropertyName("updateBy")]
    public string UpdateBy { get; }
}

public record CuratedListResponse
{
    [JsonConstructor]
    public CuratedListResponse(List<TransformedCuratedListItem> logical, List<TransformedCuratedListItem> manual)
    {
        Logical = logical ?? new List<TransformedCuratedListItem>();
        Manual = manual ?? new List<TransformedCuratedListItem>();
    }

    [JsonPropertyName("logical")]
    public List<TransformedCuratedListItem>? Logical { get; }

    [JsonPropertyName("manual")]
    public List<TransformedCuratedListItem>? Manual { get; }
}

public record TransformedCuratedListItem
{
    private readonly CuratedListItem _original;

    [JsonConstructor]
    public TransformedCuratedListItem(
        string id,
        int curatedListId,
        string? curatedListCode,
        string curatedType,
        string relevantTo,
        string name,
        string hashtag,
        int? ordering,
        CuratedListSource? curatedListSource,
        DateTime? createTime,
        DateTime? updateTime,
        string updateBy)
    {
        _original = new CuratedListItem(
            id,
            idString: id,
            curatedListId,
            curatedListCode,
            curatedType,
            relevantTo,
            name,
            hashtag,
            ordering,
            curatedListSource,
            createTime,
            updateTime,
            updateBy);

        Id = id;
    }

    public TransformedCuratedListItem(CuratedListItem original, string stringId)
    {
        _original = original;
        Id = stringId;
    }

    [JsonPropertyName("id")]
    public string Id { get; }

    [JsonPropertyName("curatedListId")]
    public int CuratedListId => _original.CuratedListId;

    [JsonPropertyName("curatedListCode")]
    public string? CuratedListCode => _original.CuratedListCode;

    [JsonPropertyName("curatedType")]
    public string CuratedType => _original.CuratedType;

    [JsonPropertyName("relevantTo")]
    public string RelevantTo => _original.RelevantTo;

    [JsonPropertyName("name")]
    public string Name => _original.Name;

    [JsonPropertyName("hashtag")]
    public string Hashtag => _original.Hashtag;

    [JsonPropertyName("ordering")]
    public int? Ordering => _original.Ordering;

    [JsonPropertyName("curatedListSource")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CuratedListSource? CuratedListSource => _original.CuratedListSource;
}

public record CuratedListUpdateRequest
{
    [JsonConstructor]
    public CuratedListUpdateRequest(
        string? name,
        string? hashtag)
    {
        Name = name;
        Hashtag = hashtag;
    }

    [JsonPropertyName("name")]
    public string? Name { get; }

    [JsonPropertyName("hashtag")]
    public string? Hashtag { get; }
}
