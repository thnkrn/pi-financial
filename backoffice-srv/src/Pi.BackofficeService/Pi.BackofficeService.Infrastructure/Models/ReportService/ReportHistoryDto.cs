using Newtonsoft.Json;
using Pi.BackofficeService.Application.Models;

namespace Pi.BackofficeService.Infrastructure.Models.ReportService;

public class GetListReportHistoryRequest
{
    public int Page { get; set; }
    public int PageSize { get; set; }

    public List<string>? ReportTypes { get; set; }

    public DateOnly? DateFrom { get; set; }

    public DateOnly? DateTo { get; set; }
}

public class DownloadRequest
{
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}

public class ReportHistoryPaginateResponse
{
    public ReportHistoryPaginateResponse(List<ReportHistoryResponse> data)
    {
        Data = data;
    }

    public List<ReportHistoryResponse> Data { get; set; }
    public int Total { get; set; }
}

public class ReportHistoryResponse
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; } = null!;

    [JsonProperty("reportName")]
    public string ReportName { get; set; } = null!;

    [JsonProperty("userName")]
    public string? UserName { get; set; }
    [JsonProperty("DateFrom")]
    public DateTime? DateFrom { get; init; }
    [JsonProperty("DateTo")]
    public DateTime? DateTo { get; init; }
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }
    [JsonProperty("updatedAt")]
    public DateTime? UpdatedAt { get; set; }
}
