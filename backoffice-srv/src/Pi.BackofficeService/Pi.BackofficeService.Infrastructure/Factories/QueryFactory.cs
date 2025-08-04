using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Infrastructure.Models.ReportService;

namespace Pi.BackofficeService.Infrastructure.Factories;

public static class QueryFactory
{
    public static ReportHistory NewReportHistory(ReportHistoryResponse reportResponse)
    {
        return new ReportHistory
        {
            Id = reportResponse.Id,
            Status = NewReportStatus(reportResponse.Status),
            Type = NewReportType(reportResponse.ReportName),
            Name = reportResponse.ReportName,
            UserName = reportResponse.UserName,
            CreatedAt = reportResponse.CreatedAt,
            UpdatedAt = reportResponse.UpdatedAt,
            DateFrom = reportResponse.DateFrom != null ? DateOnly.FromDateTime((DateTime)reportResponse.DateFrom) : null,
            DateTo = reportResponse.DateTo != null ? DateOnly.FromDateTime((DateTime)reportResponse.DateTo) : null,
            GeneratedAt = reportResponse.UpdatedAt
        };
    }

    public static ReportStatus NewReportStatus(string reportStatus)
    {
        return reportStatus.ToLower() switch
        {
            "failed" => ReportStatus.Fail,
            "done" => ReportStatus.Success,
            _ => ReportStatus.Processing
        };
    }

    public static ReportType? NewReportType(string reportName)
    {
        return reportName switch
        {
            "Deposit/Withdraw Reconcile Report (ALL)" => ReportType.AllDWReconcile,
            "Pending Transaction Report" => ReportType.PendingTransaction,
            "Velexa Transactions" => ReportType.VelexaEODTransaction,
            "Bloomberg Equity Closeprice" => ReportType.BloombergEOD,
            "Velexa Trades" => ReportType.VelexaEODTrade,
            "Velexa Account Summary" => ReportType.VelexaEODAccountSummary,
            "Velexa Trades Vat" => ReportType.VelexaEODTradeVAT,
            _ => null
        };
    }
}
