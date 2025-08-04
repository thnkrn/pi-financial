using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Pi.FundMarketData.DomainModels;
using Pi.FundMarketData.Worker.Services.FundMarket.FundConnext.Models;
using Polly;
using Polly.Retry;
using Fee = Pi.FundMarketData.Worker.Services.FundMarket.FundConnext.Models.Fee;
using TradeCalendar = Pi.FundMarketData.Worker.Services.FundMarket.FundConnext.Models.TradeCalendar;

namespace Pi.FundMarketData.Worker.Services.FundMarket.FundConnext;

public class FundConnextService : IFundConnextService
{
    private readonly HttpClient _httpClient;
    private readonly FundConnextOptions _options;
    private readonly ILogger _logger;

    private readonly AsyncRetryPolicy _retryPolicy = Policy
        .Handle<HttpRequestException>()
        .Or<TimeoutException>()
        .Or<TaskCanceledException>()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    public FundConnextService(HttpClient httpClient, IOptions<FundConnextOptions> options,
        ILogger<FundConnextService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task Authenticate(CancellationToken ct)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var loginInfo = new { username = _options.Username, password = _options.Password };

        var path = "api/auth";

        try
        {
            var res = await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.PostAsJsonAsync(path, loginInfo, ct);
                response.EnsureSuccessStatusCode();
                return response;
            });

            var responseBody = await res.Content.ReadFromJsonAsync<FundConnextAuth>(cancellationToken: ct);
            var accessToken = responseBody.Access_token;
            _httpClient.DefaultRequestHeaders.Add("x-auth-token", accessToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "FundConnextAuthenticationFailed {ex.StatusCode} {ex.Message}", ex.StatusCode,
                ex.Message);
            throw;
        }
    }

    private async Task<IEnumerable<T>> GetDataAsync<T>(DateTime businessDate, string fileName,
        Func<string[], string, T> mapper, CancellationToken ct)
    {
        var formattedDate = businessDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

        HttpResponseMessage res = new();

        try
        {
            res = await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.GetAsync($"api/files/{formattedDate}/{fileName}", ct);
                response.EnsureSuccessStatusCode();
                return response;
            });
        }
        catch (HttpRequestException ex)
        {
            var headerDictionary = res.Headers.ToDictionary(
                header => header.Key,
                header => string.Join(" | ", header.Value));
            var body = await res?.Content?.ReadAsStringAsync(ct);
            _logger.LogError(ex, "{fileName} on {businessDate} not found. {ex.StatusCode} {ex.Message} {header} {body}", fileName,
                businessDate.Date, ex.StatusCode, ex.Message, headerDictionary, body);
            throw;
        }

        var fileStream = await res.Content.ReadAsStreamAsync(ct);

        using var archive = new ZipArchive(fileStream, ZipArchiveMode.Read);
        var entry = archive.Entries[0];

        using var reader = new StreamReader(entry.Open());
        var header = await reader.ReadLineAsync(ct);
        var headerValues = header.Split('|');

        var results = new List<T>();

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(ct);
            var data = line.Split('|');
            var result = mapper(data, headerValues[0]);
            results.Add(result);
        }

        return results;
    }

    public Task<IEnumerable<FundProfile>> GetFundProfiles(DateTime businessDate, CancellationToken ct)
    {
        return GetDataAsync<FundProfile>(businessDate, "FundProfile.zip", FundProfile.Mapper, ct);
    }

    public Task<IEnumerable<Nav>> GetFundNavInfos(DateTime businessDate, CancellationToken ct)
    {
        return GetDataAsync<Nav>(businessDate, "Nav.zip", Nav.Mapper, ct);
    }

    public Task<IEnumerable<SwitchingMatrix>> GetSwitchingInfos(DateTime businessDate, CancellationToken ct)
    {
        return GetDataAsync<SwitchingMatrix>(businessDate, "SwitchingMatrix.zip", SwitchingMatrix.Mapper, ct);
    }

    public Task<IEnumerable<TradeCalendar>> GetTradeCalendars(DateTime businessDate, CancellationToken ct)
    {
        return GetDataAsync<TradeCalendar>(businessDate, "TradeCalendar.zip", TradeCalendar.Mapper, ct);
    }

    public Task<IEnumerable<FundHoliday>> GetFundHolidays(DateTime businessDate, CancellationToken ct)
    {
        return GetDataAsync<FundHoliday>(businessDate, "FundHoliday.zip", FundHoliday.Mapper, ct);
    }

    public Task<IEnumerable<Fee>> GetFeeInfos(DateTime businessDate, CancellationToken ct)
    {
        return GetDataAsync<Fee>(businessDate, "Fee.zip", Fee.Mapper, ct);
    }

    public Task<IEnumerable<FundMapping>> GetFundMappings(DateTime businessDate, CancellationToken ct)
    {
        return GetDataAsync<FundMapping>(businessDate, "FundMapping.zip", FundMapping.Mapper, ct);
    }
}
