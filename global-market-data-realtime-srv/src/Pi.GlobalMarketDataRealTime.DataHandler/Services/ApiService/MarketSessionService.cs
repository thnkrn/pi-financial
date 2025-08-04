using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using NCrontab;
using Newtonsoft.Json;
using Pi.GlobalMarketDataRealTime.DataHandler.Interfaces;
using Pi.GlobalMarketDataRealTime.DataHandler.Models.VelexaModel;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataRealTime.Domain.Entities;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Services.ApiService;

public class MarketSessionService : BackgroundService
{
    private const int TokenExpireInSecond = 3600;
    private readonly IConfiguration _configuration;
    private readonly TimeOnly _endTime;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MarketSessionService> _logger;
    private readonly IMongoService<MarketSchedule> _marketScheduleService;
    private readonly IMongoService<MarketSessionStatus> _marketSessionStatusService;
    private readonly TimeOnly _startTime;
    private readonly IVelexaHttpApiJwtTokenGenerator _velexaHttpApiJwtTokenGenerator;
    private readonly IMongoService<WhiteList> _whitelistService;
    private string? _velexaApiBaseUrl;
    private string? _velexaApiVersion;
    private string? _velexaMarketScheduler;

    public MarketSessionService(IConfiguration configuration,
        IMongoService<WhiteList> whitelistService,
        IMongoService<MarketSchedule> marketScheduleService,
        IMongoService<MarketSessionStatus> marketSessionStatusService,
        IVelexaHttpApiJwtTokenGenerator velexaHttpApiJwtTokenGenerator,
        IHttpClientFactory httpClientFactory,
        ILogger<MarketSessionService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _whitelistService = whitelistService ?? throw new ArgumentNullException(nameof(whitelistService));
        _marketScheduleService =
            marketScheduleService ?? throw new ArgumentNullException(nameof(marketScheduleService));
        _marketSessionStatusService = marketSessionStatusService ??
                                      throw new ArgumentNullException(nameof(marketSessionStatusService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _velexaHttpApiJwtTokenGenerator = velexaHttpApiJwtTokenGenerator ??
                                          throw new ArgumentNullException(nameof(velexaHttpApiJwtTokenGenerator));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

        // Initial schedule data when service is starting
        Schedule = CrontabSchedule.Parse("0 12 * * 0", new CrontabSchedule.ParseOptions
        {
            IncludingSeconds = false
        });
        NextRun = Schedule.GetNextOccurrence(DateTime.Now);

        _startTime = TimeOnly.ParseExact(
            configuration.GetValue<string>(ConfigurationKeys.MarketSessionStartTime) ?? string.Empty,
            "hh:mm:ss",
            CultureInfo.InvariantCulture
        );
        _endTime = TimeOnly.ParseExact(
            configuration.GetValue<string>(ConfigurationKeys.MarketSessionEndTime) ?? string.Empty,
            "hh:mm:ss",
            CultureInfo.InvariantCulture
        );
    }

    public CrontabSchedule Schedule { get; private set; }
    public DateTime NextRun { get; private set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _velexaApiBaseUrl = _configuration.GetValue<string>(ConfigurationKeys.VelexaHttpApiBaseUrl) ??
                            throw new ArgumentException("Velexa API Base URL is missing.");
        _velexaApiVersion = _configuration.GetValue<string>(ConfigurationKeys.VelexaApiVersion) ??
                            throw new ArgumentException("Velexa API Version is missing.");
        _velexaMarketScheduler = "0 12 * * 0";

        Schedule = CrontabSchedule.Parse(_velexaMarketScheduler, new CrontabSchedule.ParseOptions
        {
            IncludingSeconds = false
        });
        NextRun = Schedule.GetNextOccurrence(DateTime.Now);

        var isInitialize = true;
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            if (now > NextRun)
            {
                await MarketScheduleProcess(stoppingToken);
                await MarketSessionStatusProcess();
                NextRun = Schedule.GetNextOccurrence(DateTime.Now);
            }
            else
            {
                await ServiceInitialize(isInitialize, stoppingToken);
            }

            isInitialize = false;
            await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
        }
    }

    private async Task ServiceInitialize(bool isInitialize, CancellationToken stoppingToken)
    {
        if (isInitialize)
        {
            await MarketScheduleProcess(stoppingToken);
            await MarketSessionStatusProcess();
        }
        else
        {
            var marketScheduleData = await _marketScheduleService.CountAllAsync();
            if (marketScheduleData <= 0)
                await MarketScheduleProcess(stoppingToken);

            var marketSessionStatusData = await _marketSessionStatusService.CountAllAsync();
            if (marketSessionStatusData <= 0)
                await MarketSessionStatusProcess();
        }
    }

    private async Task MarketScheduleProcess(CancellationToken stoppingToken)
    {
        ValidateConfiguration();

        var instanceConfigProfile =
            _configuration.GetValue<string>(ConfigurationKeys.InstanceConfigProfile) ?? string.Empty;
        var token = _velexaHttpApiJwtTokenGenerator.GenerateJwtToken(TokenExpireInSecond);

        // Store the token generation time to track potential expiration
        var tokenGenerationTime = DateTime.UtcNow;

        IEnumerable<WhiteList> whiteListDataList;

        if (!string.IsNullOrEmpty(instanceConfigProfile))
            whiteListDataList = await _whitelistService.GetListByFilterAsync(target =>
                !string.IsNullOrEmpty(target.InstanceConfigProfile)
                && target.InstanceConfigProfile.Equals(instanceConfigProfile, StringComparison.OrdinalIgnoreCase));
        else
            whiteListDataList = await _whitelistService.GetAllAsync();

        var whiteListToList = whiteListDataList.ToList();

        foreach (var whiteListData in whiteListToList)
        {
            var symbol = whiteListData.Symbol;
            var exchange = whiteListData.Exchange;

            if (string.IsNullOrEmpty(symbol) || string.IsNullOrEmpty(exchange))
                continue;

            // Check if token needs to be refreshed (allowing a buffer before actual expiration)
            // Refresh if 80% of token lifetime has elapsed
            if ((DateTime.UtcNow - tokenGenerationTime).TotalSeconds > TokenExpireInSecond * 0.8)
            {
                token = _velexaHttpApiJwtTokenGenerator.GenerateJwtToken(TokenExpireInSecond);
                tokenGenerationTime = DateTime.UtcNow;
            }

            await ProcessSymbol(symbol, exchange, token, stoppingToken);
        }
    }

    private async Task ProcessSymbol(string symbol, string exchange, string token, CancellationToken stoppingToken)
    {
        var client = _httpClientFactory.CreateClient("MarketSession");
        var symbolId = $"{symbol}.{exchange}";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var apiUrl = $"{_velexaApiBaseUrl}{_velexaApiVersion}/symbols/{symbolId}/schedule";
        var response = await client.GetAsync(apiUrl, stoppingToken);

        if (response.IsSuccessStatusCode)
            await HandleSuccessfulResponse(response, symbol, exchange, stoppingToken);
        else
            _logger.LogError("Error: {StatusCode} {SymbolId} from velexa API", response.StatusCode, symbolId);

        await Task.Delay(TimeSpan.FromMilliseconds(3000), stoppingToken);
    }

    private async Task HandleSuccessfulResponse(HttpResponseMessage response, string symbol, string exchange,
        CancellationToken stoppingToken)
    {
        var symbolId = $"{symbol}.{exchange}";
        try
        {
            var responseData = await response.Content.ReadAsStringAsync(stoppingToken);
            var responseObj = JsonConvert.DeserializeObject<VelexaScheduleApiResponse>(responseData);
            if (responseObj != null) await UpdateMarketSchedule(symbol, exchange, responseObj);

            _logger.LogInformation("{SymbolId} get market session successfully.", symbolId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing response for {SymbolId}", symbolId);
        }
    }

    private async Task UpdateMarketSchedule(string symbol, string exchange, VelexaScheduleApiResponse responseObj)
    {
        var marketScheduleData = await _marketScheduleService.GetListByFilterAsync(e => !string.IsNullOrEmpty(e.Symbol)
            && !string.IsNullOrEmpty(e.Exchange)
            && e.Symbol.Equals(symbol)
            && e.Exchange.Equals(exchange));

        var deleteSet = marketScheduleData.Select(e => new { symbol = e.Symbol, exchange = e.Exchange });

        foreach (var marketSchedule in deleteSet.Distinct())
            if (!string.IsNullOrEmpty(marketSchedule.symbol) && !string.IsNullOrEmpty(marketSchedule.exchange))
            {
                var deletedColumns = new Dictionary<string, object>
                {
                    { "Symbol", marketSchedule.symbol },
                    { "Exchange", marketSchedule.exchange }
                };

                await _marketScheduleService.DeleteByMultipleColumnAsync(deletedColumns);
            }

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

        if (marketScheduleList != null)
            foreach (var schedule in marketScheduleList)
                if (schedule.Exchange != null
                    && schedule.Exchange.Equals("HKEX", StringComparison.OrdinalIgnoreCase)
                    && CompareTime(schedule.UTCStartTime, _startTime)
                    && CompareTime(schedule.UTCEndTime, _endTime)
                    && schedule.MarketSession != null
                    && schedule.MarketSession.Equals("Offline", StringComparison.OrdinalIgnoreCase))
                    schedule.MarketSession = "ScheduleBreak";

        if (marketScheduleList is { Length: > 0 })
            await _marketScheduleService.CreateManyAsync(marketScheduleList);
    }

    private void ValidateConfiguration()
    {
        if (_velexaApiBaseUrl == null)
            throw new ArgumentException(nameof(_velexaApiBaseUrl));
        if (_velexaApiVersion == null)
            throw new ArgumentException(nameof(_velexaApiVersion));
        if (_velexaMarketScheduler == null)
            throw new ArgumentException(nameof(_velexaMarketScheduler));
    }

    private async Task MarketSessionStatusProcess()
    {
        var instanceConfigProfile =
            _configuration.GetValue<string>(ConfigurationKeys.InstanceConfigProfile) ?? string.Empty;

        var whiteListData = await _whitelistService.GetAllAsync();
        var marketScheduleData = await _marketScheduleService.GetAllAsync();
        var scopedWhitelistData = ScopedWhitelistData(whiteListData, instanceConfigProfile);

        // Data grouping (group by exchange)
        var marketScheduleGroup = marketScheduleData
            .Where(e => scopedWhitelistData.Any(p => e.Exchange == p.Exchange && e.Symbol == p.Symbol))
            .Select(e => new { e.Exchange, e.Symbol, e.MarketSession, e.UTCStartTime, e.UTCEndTime }).Distinct()
            .GroupBy(e => e.Exchange,
                (exchange, group) =>
                    new
                    {
                        exchange,
                        data = group.GroupBy(e => new { e.MarketSession, e.UTCStartTime, e.UTCEndTime },
                            (key, group2) => new
                            {
                                exchange,
                                marketSession = key.MarketSession,
                                utcStartTime = key.UTCStartTime,
                                utcEndTime = key.UTCEndTime,
                                symbols = string.Join(",", group2.Select(e => e.Symbol).Order().ToList())
                            }).ToList()
                    })
            .Select(e => new
            {
                e.exchange,
                e.data,
                mostSymbolMatched = e.data.GroupBy(p => p.symbols).OrderByDescending(g => g.Count()).FirstOrDefault()
                    ?.Key
            }).ToList();

        // Mismatch or not equal data validation
        var mismatchOrUnequalData = marketScheduleGroup.Select(m =>
        {
            var data = m.data.Where(p => p.symbols != m.mostSymbolMatched).Select(p => new
            {
                p.exchange,
                p.marketSession,
                p.utcStartTime,
                p.utcEndTime,
                p.symbols,
                m.mostSymbolMatched
            });
            return data.Any() ? new { m.exchange, data } : null;
        }).Where(e => e != null).ToList();

        if (mismatchOrUnequalData.Count == 0)
        {
            var importMarketSessionStatus = marketScheduleGroup.SelectMany(m =>
            {
                var data = m.data.Select(p => new MarketSessionStatus
                {
                    Exchange = m.exchange,
                    MarketSession = p.marketSession,
                    UTCStartTime = p.utcStartTime,
                    UTCEndTime = p.utcEndTime
                });

                return data;
            }).ToArray();

            await UpdateMarketSessionStatus(importMarketSessionStatus);
        }
        else
        {
            foreach (var mismatch in mismatchOrUnequalData)
            {
                if (mismatch == null)
                    continue;

                var isError = true;
                var dataValidations = marketScheduleGroup.Where(e => e.exchange == mismatch.exchange);
                if (dataValidations.Any())
                {
                    var dataValidation = dataValidations.FirstOrDefault();
                    if (dataValidation == null)
                        continue;

                    // 20% error acceptance (80% data matched each of the exchanges)
                    var acceptance = Math.Round(dataValidation.data.Count * 0.2, MidpointRounding.AwayFromZero);

                    // If total mistaken data is less than 20% error acceptance, that allows updating data to the MarketSessionStatus collection
                    if (mismatch.data.Count() < acceptance)
                    {
                        var symbols = mismatch.data.Select(e => e.symbols).ToHashSet();
                        var importMarketSessionStatus = marketScheduleGroup.SelectMany(m =>
                        {
                            var data = m.data.Select(p =>
                            {
                                if (symbols.Contains(p.symbols))
                                    return null;

                                return new MarketSessionStatus
                                {
                                    Exchange = m.exchange,
                                    MarketSession = p.marketSession,
                                    UTCStartTime = p.utcStartTime,
                                    UTCEndTime = p.utcEndTime
                                };
                            });

                            return data;
                        }).Where(e => e != null).ToArray();

                        await UpdateMarketSessionStatus(importMarketSessionStatus);
                        isError = false;
                    }
                }

                // Reported mistaken data to console (greater than 20% = error, less than 20% = warning)
                foreach (var detail in mismatch.data)
                    HandleMismatchOrUnequalData(mismatch.exchange ?? "", detail.marketSession ?? "",
                        detail.utcStartTime, detail.utcEndTime, detail.symbols, detail.mostSymbolMatched ?? "",
                        isError);
            }
        }
    }

    private static List<WhiteList> ScopedWhitelistData(IEnumerable<WhiteList> whiteListData,
        string instanceConfigProfile)
    {
        // Scoped data by profile name from setting
        whiteListData = whiteListData.Where(e =>
            e.InstanceConfigProfile != null && string.Equals(e.InstanceConfigProfile, instanceConfigProfile,
                StringComparison.InvariantCultureIgnoreCase));

        // Scoped 5% from overall data (each exchange)
        var scopedWhitelistData = new List<WhiteList>();
        foreach (var exchanges in whiteListData.Select(e => e.Exchange).ToHashSet()
                     .Select((e, i) => new { index = i, exchangeName = e }))
        {
            var dataScopedByExchange = whiteListData.Where(e => e.Exchange == exchanges.exchangeName);
            var totalData = dataScopedByExchange.Count();
            int.TryParse(
                Math.Round(totalData * 0.05, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture),
                out var dataTaked);
            dataTaked = dataTaked == 0 ? 1 : dataTaked;
            var dataSkip = RandomNumberGenerator.GetInt32(totalData - dataTaked);

            dataScopedByExchange = dataScopedByExchange.Skip(dataSkip).Take(dataTaked);

            scopedWhitelistData.AddRange(dataScopedByExchange);
        }

        return scopedWhitelistData;
    }

    private void HandleMismatchOrUnequalData(string exchange, string marketSession, DateTime? utcStartTime,
        DateTime? utcEndTime, string mismatchSymbols, string matchedSymbols, bool isError = false)
    {
        var mismatchSymbolsList = mismatchSymbols.Split(",");
        var matchedSymbolsList = matchedSymbols.Split(",");
        var mismatchSymbol = mismatchSymbolsList.Except(matchedSymbolsList);

        var exceptSymbols = string.Join(",", mismatchSymbol.Any() ? mismatchSymbol : mismatchSymbolsList);
        var errorMessage =
            $"Market Session of Exchange '{exchange}' session '{marketSession}' StartTime '{utcStartTime}' EndTime '{utcEndTime}' of symbol '{exceptSymbols}' is mismatch or not equal other sessions.";

        if (!isError)
            _logger.LogWarning("{ErrorMessage}", errorMessage);
        else
            _logger.LogError("{ErrorMessage}", errorMessage);
    }

    private async Task UpdateMarketSessionStatus(MarketSessionStatus[] marketSessionStatuses)
    {
        var exchanges = marketSessionStatuses.Select(e => e.Exchange).ToHashSet();
        var marketSessionStatusData = await _marketSessionStatusService.GetAllAsync();
        if (marketSessionStatusData.Any())
        {
            foreach (var exchange in exchanges.Distinct())
            {
                if (string.IsNullOrEmpty(exchange))
                    continue;

                var deletedColumns = new Dictionary<string, object>
                {
                    { "Exchange", exchange }
                };
                await _marketSessionStatusService.DeleteByMultipleColumnAsync(deletedColumns);

                var marketSessionStatus = marketSessionStatuses.Where(e => e.Exchange == exchange).ToArray();
                if (marketSessionStatus.Length > 0)
                {
                    await _marketSessionStatusService.CreateManyAsync(marketSessionStatus);
                    _logger.LogInformation("Updated '{Exchange}' market session status {Records} records successfully.",
                        exchange, marketSessionStatus.Length);
                }
            }
        }
        else
        {
            if (marketSessionStatuses.Length > 0)
            {
                await _marketSessionStatusService.CreateManyAsync(marketSessionStatuses);
                _logger.LogInformation(
                    "Created '{Exchange}' market session status and add data {Records} records successfully.",
                    string.Join(",", exchanges), marketSessionStatuses.Length);
            }
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