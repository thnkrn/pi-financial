using System.Net.Http.Headers;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Extensions.Options;
using Pi.FundMarketData.Utils;
using Pi.FundMarketData.Worker.Services.FundMarket.Morningstar.Models;
using Polly;
using Polly.Retry;

namespace Pi.FundMarketData.Worker.Services.FundMarket.Morningstar;

public class MorningstarService : IMorningstarService
{
    private readonly HttpClient _httpClient;
    private readonly MorningstarOptions _options;
    readonly ILogger _logger;

    private readonly FormUrlEncodedContent _loginForm;

    private string _accessCode;

    private readonly AsyncRetryPolicy _retryPolicy = Policy
        .Handle<HttpRequestException>()
        .Or<TimeoutException>()
        .Or<TaskCanceledException>()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    public MorningstarService(HttpClient httpClient, IOptions<MorningstarOptions> options, ILogger<MorningstarService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        _loginForm = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("account_code", _options.AccountCode),
            new KeyValuePair<string, string>("account_password", _options.Password)
        });
        _loginForm.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
    }

    public async Task Authenticate(CancellationToken ct)
    {
        var shouldReauthenticate = await ShouldReauthenticate(ct);
        if (!shouldReauthenticate)
            return;

        _httpClient.DefaultRequestHeaders.Clear();

        var path = $"v2/service/account/CreateAccesscode/{_options.AccessTokenLifetimeInDay}d";
        var res = await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _httpClient.PostAsync(path, _loginForm, ct);
            response.EnsureSuccessStatusCode();
            return response;
        });

        string responseBody = await res.Content.ReadAsStringAsync(ct);
        var xmlDoc = XDocument.Parse(responseBody);

        var accessCodeElement = xmlDoc.Descendants("AccessCode").FirstOrDefault();
        if (accessCodeElement != null)
            _accessCode = accessCodeElement.Value;
    }

    public async Task<bool> ShouldReauthenticate(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_accessCode))
            return true;

        _httpClient.DefaultRequestHeaders.Clear();

        var path = $"v2/service/account/AccesscodeBasicInfo/{_accessCode}";
        var res = await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _httpClient.PostAsync(path, _loginForm, ct);
            response.EnsureSuccessStatusCode();
            return response;
        });

        string responseBody = await res.Content.ReadAsStringAsync(ct);
        var xmlDoc = XDocument.Parse(responseBody);

        var expireTimeElement = xmlDoc.Descendants("ExpireTime").FirstOrDefault();
        var expireTime = DateTime.Parse(expireTimeElement!.Value);
        if (expireTime > DateTime.UtcNow.AddDays(1))
            return false;

        return true;
    }

    private async Task<IEnumerable<T>> GetManyDataAsync<T>(string endpointPath, CancellationToken ct)
    {
        var res = await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _httpClient.GetAsync(endpointPath, ct);
            response.EnsureSuccessStatusCode();
            return response;
        });

        string xmlData = await res.Content.ReadAsStringAsync(ct);

        using var reader = new StringReader(xmlData);
        var serializer = new XmlSerializer(typeof(XmlApiResponse<T>));
        var resData = (XmlApiResponse<T>)serializer.Deserialize(reader);

        return resData?.DataList?.Select(x => x.ApiData) ?? Enumerable.Empty<T>();
    }

    private async Task<T> GetDataAsync<T>(string endpointPath, CancellationToken ct)
    {
        var results = await GetManyDataAsync<T>(endpointPath, ct);

        return results.FirstOrDefault();
    }

    public Task<IEnumerable<FundBasicInfo>> GetFundBasicInfos(CancellationToken ct) =>
        GetManyDataAsync<FundBasicInfo>(Path.Combine(_options.GetFundBasicInfosServiceUrl, $"universeid/{_options.UniverseId}?accesscode={_accessCode}"), ct);

    public Task<IEnumerable<FundPerformance>> GetFundPerformances(CancellationToken ct) =>
        GetManyDataAsync<FundPerformance>(Path.Combine(_options.GetFundPerformancesServiceUrl, $"universeid/{_options.UniverseId}?accesscode={_accessCode}"), ct);

    public Task<IEnumerable<RegionalAllocation>> GetRegionalAllocations(CancellationToken ct) =>
        GetManyDataAsync<RegionalAllocation>(Path.Combine(_options.GetRegionalAllocationsServiceUrl, $"universeid/{_options.UniverseId}?accesscode={_accessCode}"), ct);

    public Task<IEnumerable<FeeAndExpense>> GetFeesAndExpenses(CancellationToken ct) =>
        GetManyDataAsync<FeeAndExpense>(Path.Combine(_options.GetFeesAndExpensesServiceUrl, $"universeid/{_options.UniverseId}?accesscode={_accessCode}"), ct);

    public Task<IEnumerable<StockSectorAllocation>> GetStockSectorAllocations(CancellationToken ct) =>
        GetManyDataAsync<StockSectorAllocation>(Path.Combine(_options.GetStockSectorAllocationsServiceUrl, $"universeid/{_options.UniverseId}?accesscode={_accessCode}"), ct);

    public Task<IEnumerable<AssetClassAllocation>> GetAssetClassAllocations(CancellationToken ct) =>
        GetManyDataAsync<AssetClassAllocation>(Path.Combine(_options.GetAssetClassAllocationsServiceUrl, $"universeid/{_options.UniverseId}?accesscode={_accessCode}"), ct);

    public Task<UnderlyingHolding> GetTop25UnderlyingHolding(string mstarId, CancellationToken ct) =>
        GetDataAsync<UnderlyingHolding>(Path.Combine(_options.GetTop25UnderlyingHoldingServiceUrl, $"mstarid/{mstarId}?accesscode={_accessCode}"), ct);

    public Task<HistoricalDistribution> GetHistoricalDistribution(string mstarId, DateTime startDate, DateTime endDate, CancellationToken ct) =>
        GetDataAsync<HistoricalDistribution>(Path.Combine(_options.GetHistoricalDistributionServiceUrl,
            $"mstarid/{mstarId}?accesscode={_accessCode}&startdate={startDate.ConvertToString()}&enddate={endDate.ConvertToString()}"), ct);

    public async Task<HistoricalNav> GetHistoricalNav(string mstarId, DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        var endpointPath = Path.Combine(_options.GetHistoricalNavsServiceUrl,
            $"mstarid/{mstarId}?accesscode={_accessCode}&startdate={startDate.ConvertToString()}&enddate={endDate.ConvertToString()}");

        var res = await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _httpClient.GetAsync(endpointPath, ct);
            response.EnsureSuccessStatusCode();
            return response;
        });

        string xmlData = await res.Content.ReadAsStringAsync(ct);

        using var reader = new StringReader(xmlData);
        var serializer = new XmlSerializer(typeof(XmlResponse<HistoricalNav>));
        var resData = (XmlResponse<HistoricalNav>)serializer.Deserialize(reader);

        return resData?.DataList?.FirstOrDefault();
    }

}
