using Pi.BackofficeService.Application.Models;
using Pi.Common.Http;

namespace Pi.BackofficeService.API.Models;

public record ReportRequest
{
    public required ReportType Type { get; init; }
    public required DateOnly DateFrom { get; init; }
    public required DateOnly DateTo { get; init; }
}

public class ReportPaginateQuery : PaginateQuery
{
    public ReportGeneratedType? GeneratedType { get; set; }

    public ReportType? ReportType { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}

public record ReportResponse
{
    public required Guid Id { get; init; }
    public required string? Status { get; init; }
    public required string? Name { get; init; }
    public required ReportType? Type { get; init; }
    public required string? UserName { get; init; }
    public required DateOnly? DateFrom { get; init; }
    public required DateOnly? DateTo { get; init; }
    public required DateTime? GeneratedAt { get; init; }
    public required DateTime CreatedAt { get; init; }
}
