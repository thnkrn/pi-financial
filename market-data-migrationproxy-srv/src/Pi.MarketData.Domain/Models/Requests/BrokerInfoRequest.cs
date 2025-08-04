using Newtonsoft.Json;
namespace Pi.MarketData.Domain.Models.Request;

public class BrokerData
{
    [JsonProperty("brokerCode")]
    public string? BrokerCode { get; set; }

    [JsonProperty("brokerId")]
    public string? BrokerId { get; set; }
}

public class BrokerInfoRequest
{
    [JsonProperty("asOfDate")]
    public DateTime AsOfDate { get; set; }

    [JsonProperty("data")]
    public List<BrokerData>? Data { get; set; }
}