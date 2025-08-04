using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.NonRealTimeDataHandler.Helpers;
using Pi.GlobalMarketDataRealTime.DataHandler.Models.Velexa;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.Services;

public class MarketSessionFetcherService : BackgroundService
{
    private const int MaxRequestsPerMinute = 200;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly TimeOnly _endTime;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MarketSessionFetcherService> _logger;
    private readonly List<string> _lstExchange;
    private readonly MongoDbServices _mongoDbServices;
    private readonly SemaphoreSlim _rateLimitSemaphore;

    // ReSharper disable once CollectionNeverUpdated.Local
    private readonly ConcurrentQueue<Func<Task>> _requestQueue;

    private readonly TimeOnly _startTime;
    private readonly string _velexaApiBaseUrl;
    private readonly string _velexaApiVersion;
    private readonly IVelexaHttpApiJwtTokenGenerator _velexaHttpApiJwtTokenGenerator;
    private readonly int _velexaTokenExpireInSec;
    private string _velexaToken;

    public MarketSessionFetcherService(
        IConfiguration configuration,
        MongoDbServices mongoDbServices,
        IVelexaHttpApiJwtTokenGenerator velexaHttpApiJwtTokenGenerator,
        IHttpClientFactory httpClientFactory,
        IHostApplicationLifetime appLifetime,
        ILogger<MarketSessionFetcherService> logger
    )
    {
        _mongoDbServices = mongoDbServices;
        _velexaHttpApiJwtTokenGenerator = velexaHttpApiJwtTokenGenerator;
        _httpClientFactory = httpClientFactory;
        _appLifetime = appLifetime;
        _logger = logger;

        _velexaApiBaseUrl = configuration.GetValue<string>(ConfigurationKeys.VelexaHttpApiBaseUrl) ??
                            throw new ArgumentException("Velexa API Base URL is missing.");
        _velexaApiVersion = configuration.GetValue<string>(ConfigurationKeys.VelexaApiVersion) ??
                            throw new ArgumentException("Velexa API Version is missing.");
        _velexaTokenExpireInSec = configuration.GetValue(ConfigurationKeys.VelexaTokenExpireInSec, 600);
        _velexaToken = configuration.GetValue<string>(ConfigurationKeys.VelexaLiveToken) ?? string.Empty;

        _lstExchange = ["NYSE", "NASDAQ", "HKEX", "ARCA", "BATS"];
        // Rate limiting initialization
        _rateLimitSemaphore = new SemaphoreSlim(MaxRequestsPerMinute);
        _requestQueue = new ConcurrentQueue<Func<Task>>();

        _startTime = TimeOnly.ParseExact(
            configuration.GetValue(ConfigurationKeys.MarketSessionStartTime, "04:00:00") ?? "04:00:00",
            "hh:mm:ss",
            CultureInfo.InvariantCulture
        );
        _endTime = TimeOnly.ParseExact(
            configuration.GetValue(ConfigurationKeys.MarketSessionEndTime, "05:00:00") ?? "05:00:00",
            "hh:mm:ss",
            CultureInfo.InvariantCulture
        );

        StartRequestProcessor();
    }

    private void StartRequestProcessor()
    {
        _ = Task.Run(async () =>
        {
            while (true)
                if (_requestQueue.TryDequeue(out var requestAction))
                    try
                    {
                        await _rateLimitSemaphore.WaitAsync();
                        await requestAction();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing queued request");
                    }
                    finally
                    {
                        _rateLimitSemaphore.Release();
                        await Task.Delay(TimeSpan.FromSeconds(60.0 / MaxRequestsPerMinute));
                    }
                else
                    await Task.Delay(250);
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Process(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while processing");
        }
        finally
        {
            _appLifetime.StopApplication();
        }
    }

    private async Task Process(CancellationToken stoppingToken)
    {
        ArgumentNullException.ThrowIfNull(_velexaApiBaseUrl);
        ArgumentNullException.ThrowIfNull(_velexaApiVersion);

        var whiteListDataList =
            await _mongoDbServices.WhiteListService.GetAllByFilterAsync(target => target.IsWhitelist);
        foreach (var whiteListData in whiteListDataList)
        {
            if (string.IsNullOrEmpty(whiteListData.Symbol) || string.IsNullOrEmpty(whiteListData.Exchange))
                continue;

            if (string.IsNullOrEmpty(_velexaToken) || _velexaHttpApiJwtTokenGenerator.IsTokenExpired(_velexaToken))
                _velexaToken = _velexaHttpApiJwtTokenGenerator.GenerateJwtToken(_velexaTokenExpireInSec);

            var symbol = whiteListData.Symbol;
            var exchange = whiteListData.Exchange;
            await ProcessSymbol(symbol, exchange, _velexaToken, stoppingToken);
        }

        await UpdateMarketSessionStatus();
    }

    private async Task ProcessSymbol(string symbol, string exchange, string token, CancellationToken stoppingToken)
    {
        using var client = _httpClientFactory.CreateClient("MarketSession");
        var symbolId = WebUtility.UrlEncode($"{symbol}.{exchange}");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var apiUrl = $"{_velexaApiBaseUrl}/md/{_velexaApiVersion}/symbols/{symbolId}/schedule";
        var response = await client.GetAsync(apiUrl, stoppingToken);
        if (response.IsSuccessStatusCode)
            await HandleSuccessfulResponse(response, symbol, exchange, stoppingToken);
        else
            _logger.LogError("Error: {StatusCode} {SymbolId} from velexa API", response.StatusCode, symbolId);
    }

    private async Task HandleSuccessfulResponse(HttpResponseMessage response, string symbol, string exchange,
        CancellationToken stoppingToken)
    {
        var symbolId = $"{symbol}.{exchange}";
        try
        {
            var responseData = await response.Content.ReadAsStringAsync(stoppingToken);
            var responseObj = JsonConvert.DeserializeObject<VelexaScheduleApiResponse>(responseData);
            if (responseObj != null)
            {
                _logger.LogInformation("{Count} Market schedule received for symbol {Symbol}.",
                    responseObj.Intervals?.Count, symbolId);
                await UpdateMarketSchedule(symbol, exchange, responseObj);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing response for {SymbolId}", symbolId);
        }
    }

    private async Task UpdateMarketSessionStatus()
    {
        // Loop on exchange list
        foreach (var exchange in _lstExchange)
        {
            var scheduleByVenue = await _mongoDbServices.MarketScheduleService.GetAllByFilterAsync(target =>
                target.Exchange == exchange);
            var scheduleToCalculate = scheduleByVenue.OrderByDescending(x => x.UTCEndTime).DistinctBy(x => x.Symbol)
                .Take(15).ToList();
            if (scheduleToCalculate.Count == 0)
                continue;
            await GetMostCommon(scheduleToCalculate, exchange);
        }
    }

    private async Task GetMostCommon(List<MarketSchedule> lstSchedule, string exchange)
    {
        var mostCommonByStartTime = lstSchedule
            .GroupBy(x => new { x.UTCStartTime, x.MarketSession })
            .ToDictionary(
                group => group.Key,
                group => group.GroupBy(item => item.Symbol)
                    .OrderByDescending(symbol => symbol.Count())
                    .FirstOrDefault()?
                    .Key ?? string.Empty
            ).FirstOrDefault();
        await ReplaceMarketSessionStatus(mostCommonByStartTime.Value, exchange);
    }

    private async Task ReplaceMarketSessionStatus(string sourceSymbol, string exchange)
    {
        _logger.LogInformation("Updating MarketSessionStatus with the template of {Symbol}", sourceSymbol);
        var marketScheduleList =
            await _mongoDbServices.MarketScheduleService.GetAllByFilterAsync(target => target.Symbol == sourceSymbol);
        var marketSchedules = marketScheduleList.ToList();
        var marketSessionStatusList = marketSchedules.Select(x => new MarketSessionStatus
        {
            Exchange = x.Exchange,
            MarketSession = x.MarketSession,
            UTCEndTime = x.UTCEndTime,
            UTCStartTime = x.UTCStartTime
        });
        _logger.LogInformation("{Count} sessions update for exchange : {Exchange}", marketSchedules.Count,
            exchange);
        await _mongoDbServices.MarketSessionStatusService.BulkUpsertAsync(marketSessionStatusList,
            target => target.Exchange == exchange);
    }

    [SuppressMessage("SonarQube", "S3776")]
    private async Task UpdateMarketSchedule(string symbol, string exchange, VelexaScheduleApiResponse responseObj)
    {
        if (string.IsNullOrEmpty(symbol) || string.IsNullOrEmpty(exchange) || responseObj.Intervals == null)
        {
            _logger.LogWarning(
                "Invalid inputs for UpdateMarketSchedule: Symbol={Symbol}, Exchange={Exchange}, HasIntervals={HasIntervals}",
                symbol, exchange, responseObj.Intervals != null);
            return;
        }

        try
        {
            var marketScheduleList = responseObj.Intervals?.Select(item => new MarketSchedule
            {
                Symbol = symbol,
                Exchange = exchange,
                MarketSession = item.Name,
                UTCStartTime = item.Period?.Start != null
                    ? DateTimeOffset.FromUnixTimeMilliseconds(item.Period.Start.Value).UtcDateTime
                    : default,
                UTCEndTime = item.Period?.End != null
                    ? DateTimeOffset.FromUnixTimeMilliseconds(item.Period.End.Value).UtcDateTime
                    : default
            }).ToArray();

            // Update MarketSession value for HKEX exchange
            if (marketScheduleList != null)
                foreach (var schedule in marketScheduleList)
                    if (!string.IsNullOrEmpty(schedule.Exchange)
                        && schedule.Exchange.Equals("HKEX", StringComparison.OrdinalIgnoreCase)
                        && CompareTime(schedule.UTCStartTime, _startTime)
                        && CompareTime(schedule.UTCEndTime, _endTime)
                        && !string.IsNullOrEmpty(schedule.MarketSession)
                        && schedule.MarketSession.Equals("Offline", StringComparison.OrdinalIgnoreCase))
                        schedule.MarketSession = "ScheduleBreak";

            if (marketScheduleList != null && marketScheduleList.Any())
                await _mongoDbServices.MarketScheduleService.BulkUpsertAsync(marketScheduleList,
                    target => target.Symbol == symbol);
        }
        catch (Exception ex)
        {
            // Log error and handle exception
            _logger.LogError(ex, "Error updating market schedule for {Symbol}", symbol);
        }
    }

    private static bool CompareTime(DateTime? date, TimeOnly time)
    {
        if (date.HasValue)
        {
            var timeFromDateTime = new TimeOnly(date.Value.Hour, date.Value.Minute, date.Value.Second);
            return timeFromDateTime == time;
        }

        return false;
    }
}