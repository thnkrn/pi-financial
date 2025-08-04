using System.Text.Json.Serialization;

namespace Pi.Financial.FundService.Infrastructure.Models;

public record FundConnextErrorResponse
{
    [JsonPropertyName("errMsg")]
    public FundConnextError? ErrMsg { get; set; }
}

public record FundConnextError
{
    public string? code { get; set; }
    public string? message { get; set; }
}
