using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Models.Commons;
using Pi.BackofficeService.Application.Queries.Filters;

namespace Pi.BackofficeService.Application.Services.ReportService;

public record GenerateReportPayload
{
    public required Guid Id { get; init; }
    public required ReportType ReportType { get; init; }
    public required string UserName { get; init; }
    public required DateOnly DateFrom { get; init; }
    public required DateOnly DateTo { get; init; }
}

public interface IReportService
{
    Task<bool> Generate(GenerateReportPayload payload);
    Task<PaginateResult<ReportHistory>> GetReportHistories(int page, int pageSize, string? orderBy, string? orderDir, ReportFilter reportFilter);
    Task<string?> GetFileUrl(Guid reportId);
    Task<List<ReportType>> GetReportTypesByGeneratedTypes(ReportGeneratedType generatedType);
    Task<byte[]> DownloadDepositWithdrawDailyReport(DateOnly dateFrom, DateOnly dateTo, ReportType reportType);
}
