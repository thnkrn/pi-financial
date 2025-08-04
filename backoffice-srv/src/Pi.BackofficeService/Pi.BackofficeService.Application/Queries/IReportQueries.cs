
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Models.Commons;
using Pi.BackofficeService.Application.Queries.Filters;

namespace Pi.BackofficeService.Application.Queries;

public interface IReportQueries
{
    Task<PaginateResult<ReportHistory>> GetPaginateReportHistories(int page, int pageSize, string? orderBy, string? orderDir, ReportFilter reportFilter);
    Task<string?> GetUrl(Guid reportId);
    Task<List<ReportType>> GetReportTypes(ReportGeneratedType? generatedType);
}
