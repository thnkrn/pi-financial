using Pi.BackofficeService.Application.Services.ReportService;
using Pi.BackofficeService.Application.Utils;
using Pi.BackofficeService.Infrastructure.Models.ReportService;

namespace Pi.BackofficeService.Infrastructure.Factories;

public static class ServiceFactory
{
    public static ReportGenerateRequest NewReportGenerateRequest(GenerateReportPayload payload)
    {
        var reportName = EnumUtil.GetEnumDescription(payload.ReportType) ?? payload.ReportType.ToString();

        return new ReportGenerateRequest
        {
            Id = payload.Id,
            ReportName = reportName,
            UserName = payload.UserName,
            DateFrom = payload.DateFrom,
            DateTo = payload.DateTo
        };
    }
}
