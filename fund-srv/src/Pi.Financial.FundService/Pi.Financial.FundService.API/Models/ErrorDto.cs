namespace Pi.Financial.FundService.API.Models;

public record ErrorResponse
{
    public required string ErrorCode { get; set; }
    public required string Message { get; set; }
    public int? Status { get; set; }
    public string? DebugMessage { get; set; }
}
