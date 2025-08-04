namespace Pi.BackofficeService.Infrastructure.Models.ReportService;

public record ReportGetUrlRequest
{
    public required string Bucket { get; set; }
    public required string Url { get; set; }
}

public record ReportGetUrlResponse
{
    public required string Url { get; set; }
}

public record DownloadReportResponse
{
    public required string csvData { get; set; }
}
