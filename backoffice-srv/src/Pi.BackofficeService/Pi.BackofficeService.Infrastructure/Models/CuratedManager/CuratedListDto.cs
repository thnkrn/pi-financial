using System.Text.Json.Serialization;
using Pi.BackofficeService.Application.Models;

namespace Pi.BackofficeService.Infrastructure.CuratedManager;

public record CuratedListThirdPartyApiResponse
{
    [JsonConstructor]
    public CuratedListThirdPartyApiResponse(CuratedListData? set, CuratedListData? ge)
    {
        Set = set;
        Ge = ge;
    }

    [JsonPropertyName("set")]
    public CuratedListData? Set { get; }

    [JsonPropertyName("ge")]
    public CuratedListData? Ge { get; }
}

public record CuratedListData
{
    [JsonConstructor]
    public CuratedListData(List<CuratedListItem> data)
    {
        Data = data ?? new List<CuratedListItem>();
    }

    [JsonPropertyName("data")]
    public List<CuratedListItem> Data { get; }
}

public record UpdateCuratedListByIdThirdPartyResponse
{
    [JsonConstructor]
    public UpdateCuratedListByIdThirdPartyResponse(CuratedListUpdateData? set, CuratedListUpdateData? ge)
    {
        Set = set;
        Ge = ge;
    }

    [JsonPropertyName("set")]
    public CuratedListUpdateData? Set { get; }

    [JsonPropertyName("ge")]
    public CuratedListUpdateData? Ge { get; }
}

public record CuratedListUpdateData
{
    [JsonConstructor]
    public CuratedListUpdateData(bool success, CuratedListItem? updatedCuratedList)
    {
        Success = success;
        UpdatedCuratedList = updatedCuratedList;
    }

    [JsonPropertyName("success")]
    public bool Success { get; }

    [JsonPropertyName("updatedCuratedList")]
    public CuratedListItem? UpdatedCuratedList { get; }
}