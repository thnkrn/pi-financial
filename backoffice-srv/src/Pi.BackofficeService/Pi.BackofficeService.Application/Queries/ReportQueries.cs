using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Models.Commons;
using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Application.Services.ReportService;

namespace Pi.BackofficeService.Application.Queries;

public class ReportQueries : IReportQueries
{
    private readonly IReportService _reportService;

    public ReportQueries(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<PaginateResult<ReportHistory>> GetPaginateReportHistories(int page, int pageSize, string? orderBy, string? orderDir, ReportFilter reportFilter)
    {
        var response = await _reportService.GetReportHistories(page, pageSize, orderBy, orderDir, reportFilter);

        return new PaginateResult<ReportHistory>(response.Records, response.Page, response.PageSize, response.Total, response.OrderBy, response.OrderDir);
    }

    public async Task<string?> GetUrl(Guid reportId)
    {
        return await _reportService.GetFileUrl(reportId);
    }

    public async Task<List<ReportType>> GetReportTypes(ReportGeneratedType? generatedType)
    {
        if (generatedType == null)
        {
            return Enum.GetValues(typeof(ReportType)).Cast<ReportType>().ToList();
        }

        return await _reportService.GetReportTypesByGeneratedTypes((ReportGeneratedType)generatedType);
    }
}
