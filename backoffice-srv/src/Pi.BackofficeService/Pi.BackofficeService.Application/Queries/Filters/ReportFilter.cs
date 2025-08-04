using Pi.BackofficeService.Application.Models;

namespace Pi.BackofficeService.Application.Queries.Filters;

public class ReportFilter
{
    public ReportGeneratedType? GeneratedTypes { get; set; }

    public ReportType? ReportType { get; set; }

    public DateOnly? DateFrom { get; set; }

    public DateOnly? DateTo { get; set; }
}