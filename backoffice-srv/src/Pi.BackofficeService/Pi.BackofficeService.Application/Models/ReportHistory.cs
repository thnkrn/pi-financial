namespace Pi.BackofficeService.Application.Models;

public record ReportHistory
{
    public required Guid Id { get; init; }
    public required ReportStatus Status { get; init; }
    public required string? Name { get; init; }
    public required ReportType? Type { get; init; }
    public required string? UserName { get; init; }
    public required DateOnly? DateFrom { get; init; }
    public required DateOnly? DateTo { get; init; }
    public required DateTime? GeneratedAt { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? UpdatedAt { get; init; }
}
