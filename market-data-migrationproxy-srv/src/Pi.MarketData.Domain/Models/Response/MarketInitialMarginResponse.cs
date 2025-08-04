using System.Text.Json.Serialization;
namespace Pi.MarketData.Domain.Models.Response;

public class MarketInitialMarginResponse
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("data")]
    public object Data { get; set; } = new object();

    [JsonPropertyName("error")]
    public object Error { get; set; } = new object();
}

public class InitialMarginResponse
{
    [JsonPropertyName("totalProcessed")]
    public string? TotalProcessed { get; set; }

    [JsonPropertyName("asOfDate")]
    public DateTime? AsOfDate { get; set; }
}

public class InitialMarginErrorResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
