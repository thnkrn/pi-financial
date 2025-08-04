using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Models.Commons;
using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Application.Services.ReportService;
using Pi.BackofficeService.Application.Utils;
using Pi.BackofficeService.Infrastructure.Factories;
using Pi.BackofficeService.Infrastructure.Models.ReportService;

namespace Pi.BackofficeService.Infrastructure.Services;

public class ReportService : IReportService
{
    private const string XApiKey = "x-api-key";
    private readonly HttpClient _client;
    private readonly string _baseUrl;
    private readonly string _apiKey;
    private readonly ILogger<ReportService> _logger;

    public ReportService(HttpClient client, string baseUrl, string apiKey, ILogger<ReportService> logger)
    {
        _client = client;
        _baseUrl = baseUrl;
        _apiKey = apiKey;
        _logger = logger;
    }

    public async Task<bool> Generate(GenerateReportPayload payload)
    {
        var requestPayload = ServiceFactory.NewReportGenerateRequest(payload);

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/report/success-dw-reconcile");
        request.Headers.Add(XApiKey, _apiKey);
        request.Content = GenerateRequestContent(requestPayload);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var reportCreated = JsonConvert.DeserializeObject<ReportGenerateResponse>(await response.Content.ReadAsStringAsync());

        return reportCreated?.ExecutionArn != null;
    }

    public async Task<PaginateResult<ReportHistory>> GetReportHistories(int page, int pageSize, string? orderBy, string? orderDir, ReportFilter reportFilter)
    {

        var payload = new GetListReportHistoryRequest
        {
            Page = page,
            PageSize = pageSize,
            ReportTypes = await GetReportNames(reportFilter),
            DateFrom = reportFilter.DateFrom,
            DateTo = reportFilter.DateTo
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/list-report-history");
        request.Headers.Add(XApiKey, _apiKey);
        request.Content = GenerateRequestContent(payload);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var reportResponse = JsonConvert.DeserializeObject<ReportHistoryPaginateResponse>(await response.Content.ReadAsStringAsync());

        if (reportResponse == null)
        {
            return new PaginateResult<ReportHistory>(new List<ReportHistory>(), page, pageSize, 0, orderBy, orderDir);
        }

        var records = reportResponse.Data.Select(QueryFactory.NewReportHistory).ToList();

        return new PaginateResult<ReportHistory>(records, page, pageSize, reportResponse.Total, orderBy, orderDir);
    }

    public async Task<string?> GetFileUrl(Guid reportId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/report/{reportId.ToString()}/presigned-url");
        request.Headers.Add(XApiKey, _apiKey);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responsePayload = JsonConvert.DeserializeObject<ReportGetUrlResponse>(await response.Content.ReadAsStringAsync());

        return responsePayload?.Url;
    }

    private static StringContent GenerateRequestContent<T>(T payload)
    {
        var contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        var stringPayload = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.Indented
        });

        return new StringContent(stringPayload, null, "application/json");
    }

    public Task<List<ReportType>> GetReportTypesByGeneratedTypes(ReportGeneratedType generatedType)
    {
        var reportTypes = generatedType switch
        {
            ReportGeneratedType.OnDemand => new List<ReportType>() { ReportType.AllDWReconcile },
            ReportGeneratedType.Auto => new List<ReportType>() { ReportType.PendingTransaction, ReportType.BloombergEOD, ReportType.VelexaEODTrade, ReportType.VelexaEODTransaction, ReportType.VelexaEODAccountSummary, ReportType.VelexaEODTradeVAT },
            ReportGeneratedType.DepositWithdraw => new List<ReportType>() { ReportType.PiAppDepositWithdrawDailyReport, ReportType.PiAppGlobalReconcileReport, ReportType.BillPaymentReconcileReport },
            _ => new List<ReportType>()
        };

        return Task.FromResult(reportTypes);
    }

    public async Task<byte[]> DownloadDepositWithdrawDailyReport(DateOnly dateFrom, DateOnly dateTo, ReportType reportType)
    {
        var payload = new DownloadRequest
        {
            DateFrom = dateFrom,
            DateTo = dateTo
        };

        var requestUri = reportType switch
        {
            ReportType.PiAppDepositWithdrawDailyReport => $"{_baseUrl}/report/wallet/pi-app-deposit-withdraw/download",
            ReportType.PiAppGlobalReconcileReport => $"{_baseUrl}/report/wallet/global-reconcile/download",
            ReportType.BillPaymentReconcileReport => $"{_baseUrl}/report/wallet/deposit-bill-payment/download",
            _ => throw new ArgumentOutOfRangeException(nameof(reportType), reportType, null)
        };

        var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        request.Headers.Add(XApiKey, _apiKey);
        request.Content = GenerateRequestContent(payload);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        // var base64Data = await response.Content.ReadAsStringAsync();
        // var bufferData = Convert.FromBase64String(base64Data);

        // Currently, Lambda default decoded base64, So we can just read content as byte[]
        var bufferData = await response.Content.ReadAsByteArrayAsync();

        return bufferData;
    }

    private async Task<List<string>?> GetReportNames(ReportFilter reportFilter)
    {
        List<string>? reportNames;
        if (reportFilter.ReportType == null)
        {
            var reportTypes = reportFilter.GeneratedTypes != null
                ? await GetReportTypesByGeneratedTypes((ReportGeneratedType)reportFilter.GeneratedTypes)
                : null;
            reportNames = reportTypes?.Select(q => EnumUtil.GetEnumDescription(q) ?? "").ToList() ?? null;
        }
        else
        {
            reportNames = new List<string>() { EnumUtil.GetEnumDescription((ReportType)reportFilter.ReportType) ?? "" };
        }
        return reportNames;
    }
}
