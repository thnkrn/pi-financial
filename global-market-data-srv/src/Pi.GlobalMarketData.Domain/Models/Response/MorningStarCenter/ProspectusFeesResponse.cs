using Newtonsoft.Json;

namespace Pi.GlobalMarketData.Domain.Models.Response.MorningStarCenter;

public class ProspectusFeesResponse
{
    public List<Data<ProspectusFees>>? Data { get; set; }

    public static ProspectusFeesResponse? FromJson(string json)
    {
        return JsonConvert.DeserializeObject<ProspectusFeesResponse>(json);
    }
}

public class ProspectusFees
{
    public string? Name { get; set; }
    public string? ProspectusDate { get; set; }
    public string? LatestProspectusDate { get; set; }
    public string? PerformanceFee { get; set; }
    public string? ActualManagementFee { get; set; }
    public string? NetExpenseRatio { get; set; }
    public string? GrossExpenseRatio { get; set; }
    public string? PerformanceFeeIndexName { get; set; }
    public string? PerformanceFeeIndexWeighting { get; set; }
}
