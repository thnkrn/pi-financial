namespace Pi.BackofficeService.Infrastructure.Models.ReportService;

public record ReportGenerateRequest
{
    public required Guid Id { get; init; }
    public required string ReportName { get; init; }
    public required string UserName { get; init; }
    public required DateOnly DateFrom { get; init; }
    public required DateOnly DateTo { get; init; }
}

public record ReportGenerateResponse
{
    public required string ExecutionArn { get; set; }
    public required float StartDate { get; set; }
}
