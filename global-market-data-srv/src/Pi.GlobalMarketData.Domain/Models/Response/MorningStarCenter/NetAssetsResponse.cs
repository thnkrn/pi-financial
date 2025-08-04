using Newtonsoft.Json;

namespace Pi.GlobalMarketData.Domain.Models.Response.MorningStarCenter;

public class NetAssetsResponse
{
    public List<Data<NetAssets>>? Data { get; set; }

    public static NetAssetsResponse? FromJson(string json) =>
        JsonConvert.DeserializeObject<NetAssetsResponse>(json);
}

public class NetAssets
{
    public string? Name { get; set; }
    public string? NetAssetsDate { get; set; }
    public string? ShareClassNetAssets { get; set; }
    public string? AsOfOriginalReportedDate { get; set; }
    public string? AsOfOriginalReported { get; set; }
    public string? AsOfOriginalReportedCurrencyId { get; set; }
}
