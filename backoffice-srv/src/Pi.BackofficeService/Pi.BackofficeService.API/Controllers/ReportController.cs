using System.Net.Mime;
using MassTransit;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi.ActivityService.IntegrationEvents;
using Pi.BackofficeService.API.Factories;
using Pi.BackofficeService.API.Models;
using Pi.BackofficeService.API.Startup;
using Pi.BackofficeService.Application.Commands.User;
using Pi.BackofficeService.Application.Factories;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Application.Services.ReportService;
using Pi.BackofficeService.Domain.AggregateModels.User;
using Pi.Common.Http;

namespace Pi.BackofficeService.API.Controllers;

[Authorize("ReportRead")]
[ApiController]
[Route("reports")]
public class ReportController : ControllerBase
{
    private readonly IReportQueries _reportQueries;
    private readonly IReportService _reportService;
    private readonly IBus _bus;

    public ReportController(IReportQueries reportQueries, IReportService reportService, IBus bus)
    {
        _reportQueries = reportQueries;
        _reportService = reportService;
        _bus = bus;
    }

    [HttpGet("types")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<NameAliasResponse<ReportType>>>))]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> GetTypes([FromQuery] ReportTypeQuery reportTypeQuery)
    {
        var report = await _reportQueries.GetReportTypes(reportTypeQuery.generatedType);
        var response = report.Select(DtoFactory.NewGenericNameAliasResponse).ToList();

        return Ok(new ApiResponse<List<NameAliasResponse<ReportType>>>(response));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiPaginateResponse<List<ReportResponse>>))]
    public async Task<IActionResult> Get([FromQuery] ReportPaginateQuery query)
    {

        var reportFilter = new ReportFilter
        {
            GeneratedTypes = query.GeneratedType,
            ReportType = query.ReportType,
            DateFrom = query.DateFrom,
            DateTo = query.DateTo,
        };

        var result = await _reportQueries.GetPaginateReportHistories(query.Page, query.PageSize, query.OrderBy, query.OrderDir, reportFilter);

        return Ok(new ApiPaginateResponse<List<ReportResponse>>(result.Records.Select(DtoFactory.NewReportResponse).ToList(),
            query.Page,
            query.PageSize,
            result.Total,
            query.OrderBy,
            query.OrderDir
        ));
    }

    [HttpPost]
    [Authorize("ReportExport")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Generate([FromBody] ReportRequest payload)
    {
        var fullname = User.FindFirstValue("first_name") + " " + User.FindFirstValue("last_name")!;
        var report = new GenerateReportPayload
        {
            Id = Guid.NewGuid(),
            ReportType = payload.Type,
            UserName = fullname,
            DateFrom = payload.DateFrom,
            DateTo = payload.DateTo
        };

        var result = await _reportService.Generate(report);
        var userId = await GetUserId();

        await _bus.Publish(EntityFactory
            .NewActivityEvent(userId,
                "Report Generated",
                CommonActivityType.Created.ToString(),
                refId: report.Id.ToString()));

        return result ? StatusCode(StatusCodes.Status201Created) : Problem();
    }

    [HttpGet("{reportId}/download_url")]
    [Authorize("ReportExport")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
    public async Task<IActionResult> GetUrl(Guid reportId)
    {
        var result = await _reportQueries.GetUrl(reportId);

        if (result == null) return NotFound();
        var userId = await GetUserId();
        await _bus.Publish(EntityFactory
            .NewActivityEvent(userId,
                "Report Url Generated",
                CommonActivityType.Created.ToString(),
                refId: reportId.ToString()));

        return Ok(new ApiResponse<string>(result));
    }

    [HttpGet("pi-app-dw-daily/download")]
    [Authorize("ReportExport")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
    public async Task<IActionResult> GetPiAppDwDailyReport([FromQuery] DateOnly dateFrom, [FromQuery] DateOnly dateTo, [FromQuery] ReportType? reportType)
    {
        byte[] result;
        string filePrefix;
        switch (reportType)
        {
            case ReportType.PiAppGlobalReconcileReport:
                result = await _reportService.DownloadDepositWithdrawDailyReport(dateFrom, dateTo, reportType.Value);
                filePrefix = "PI_APP_Global_Reconcile_Report";
                break;
            case ReportType.BillPaymentReconcileReport:
                result = await _reportService.DownloadDepositWithdrawDailyReport(dateFrom, dateTo, reportType.Value);
                filePrefix = "Bill_Payment_Reconcile_Report";
                break;
            case ReportType.PendingTransaction:
            case ReportType.AllDWReconcile:
            case ReportType.BloombergEOD:
            case ReportType.VelexaEODTrade:
            case ReportType.VelexaEODTransaction:
            case ReportType.VelexaEODAccountSummary:
            case ReportType.VelexaEODTradeVAT:
                return BadRequest();
            default:
                result = await _reportService.DownloadDepositWithdrawDailyReport(dateFrom, dateTo, ReportType.PiAppDepositWithdrawDailyReport);
                filePrefix = "PI_APP_DW_Daily_Report";
                break;
        }

        return File(new MemoryStream(result), MediaTypeNames.Application.Octet, $"{filePrefix}_{dateFrom}-{dateTo}.csv");
    }

    private async Task<Guid> GetUserId()
    {
        var response = await _bus.CreateRequestClient<UserUpdateOrCreateRequest>()
            .GetResponse<UserIdResponse>(new UserUpdateOrCreateRequest(
                (Guid)User.GetIamId()!,
                User.FindFirstValue("first_name")!,
                User.FindFirstValue("last_name")!,
                User.FindFirstValue(ClaimTypes.Email)!
            ));

        return response.Message.Id;
    }
}
