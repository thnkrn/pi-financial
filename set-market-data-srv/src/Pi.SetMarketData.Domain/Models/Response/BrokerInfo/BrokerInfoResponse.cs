using Newtonsoft.Json;

namespace Pi.SetMarketData.Domain.Models.Response.BrokerInfo;

public class BrokerInfoResponse
{
    [JsonProperty("status")]
    public bool Status { get; set; }
    [JsonProperty("data")]
    public ResponseData? Data { get; set; }
    [JsonProperty("error")]
    public ErrorResponse? Error { get; set; }
}

public class ResponseData
{
    [JsonProperty("totalProcessed")]
    public int TotalProcessed { get; set; }
    [JsonProperty("asOfDate")]
    public DateTime AsOfDate { get; set; }
}

public class ErrorResponse
{
    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;
}