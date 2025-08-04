using System.Text.Json.Serialization;
using Pi.BackofficeService.Application.Models;

namespace Pi.BackofficeService.Infrastructure.Models.CuratedManager;

public record CuratedMembersThirdPartyApiResponse
{
    [JsonConstructor]
    public CuratedMembersThirdPartyApiResponse(CuratedMemberData? set, CuratedMemberData? ge)
    {
        Set = set;
        Ge = ge;
    }

    [JsonPropertyName("set")]
    public CuratedMemberData? Set { get; }

    [JsonPropertyName("ge")]
    public CuratedMemberData? Ge { get; }
}

public record CuratedMemberData
{
    [JsonConstructor]
    public CuratedMemberData(List<CuratedMemberItem> data)
    {
        Data = data ?? new List<CuratedMemberItem>();
    }

    [JsonPropertyName("data")]
    public List<CuratedMemberItem> Data { get; }
}

