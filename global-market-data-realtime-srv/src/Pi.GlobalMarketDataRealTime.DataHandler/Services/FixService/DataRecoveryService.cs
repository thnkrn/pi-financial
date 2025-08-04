using System.Collections;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Pi.GlobalMarketDataRealTime.DataHandler.Interfaces;
using Pi.GlobalMarketDataRealTime.DataHandler.Models;
using Pi.GlobalMarketDataRealTime.DataHandler.Models.VelexaModel;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataRealTime.Domain.Entities;
using Pi.GlobalMarketDataRealTime.Domain.Models.Fix;
using Pi.GlobalMarketDataRealTime.Domain.Models.Kafka;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Kafka;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Services.FixService;

public class DataRecoveryService : IDataRecoveryService
{
    private const int TokenExpireInSecond = 600;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IKafkaPublisher _kafkaPublisher;
    private readonly ILogger<DataRecoveryService> _logger;
    private readonly IVelexaHttpApiJwtTokenGenerator _velexaHttpApiJwtTokenGenerator;
    private readonly IMongoService<WhiteList> _whitelistService;

    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="kafkaPublisher"></param>
    /// <param name="logger"></param>
    /// <param name="velexaHttpApiJwtTokenGenerator"></param>
    /// <param name="whitelistService"></param>
    /// <param name="httpClientFactory"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public DataRecoveryService(
        IConfiguration configuration,
        IKafkaPublisher kafkaPublisher,
        ILogger<DataRecoveryService> logger,
        IVelexaHttpApiJwtTokenGenerator velexaHttpApiJwtTokenGenerator,
        IMongoService<WhiteList> whitelistService,
        IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _kafkaPublisher = kafkaPublisher ?? throw new ArgumentNullException(nameof(kafkaPublisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _velexaHttpApiJwtTokenGenerator = velexaHttpApiJwtTokenGenerator ??
                                          throw new ArgumentNullException(nameof(velexaHttpApiJwtTokenGenerator));
        _whitelistService = whitelistService ?? throw new ArgumentNullException(nameof(whitelistService));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task RecoverData()
    {
        await RequestHistoricalDataPublicTrades();
    }

    private async Task RequestHistoricalDataPublicTrades()
    {
        var client = CreateHttpClient();
        var instanceConfigProfile = _configuration.GetValue<string>(ConfigurationKeys.InstanceConfigProfile);
        var symbolList = await GetSymbolList(instanceConfigProfile ?? string.Empty);
        var getDataTime = new DateTimeOffset(DateTime.UtcNow).AddHours(-6).ToUnixTimeMilliseconds();

        foreach (var data in symbolList)
        {
            var snapshots = new List<MarketDataSnapshot>();
            await ProcessPublicTrades(client, data, getDataTime, snapshots);
            await ProcessBidOffer(client, data, getDataTime, snapshots);
            await PublishSnapshots(snapshots, data.Symbol ?? string.Empty);
            await Task.Delay(TimeSpan.FromMilliseconds(10000));
        }
    }

    private HttpClient CreateHttpClient()
    {
        var client = _httpClientFactory.CreateClient("MarketSession");
        var token = _velexaHttpApiJwtTokenGenerator.GenerateJwtToken(TokenExpireInSecond);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }

    private async Task<List<SymbolData>> GetSymbolList(string instanceConfigProfile)
    {
        var whitelistData = await _whitelistService
            .GetListByFilterAsync(e => !string.IsNullOrEmpty(e.Exchange)
                                       && !string.IsNullOrEmpty(e.Symbol)
                                       && !string.IsNullOrEmpty(e.InstanceConfigProfile)
                                       && string.Equals(e.InstanceConfigProfile, instanceConfigProfile,
                                           StringComparison.OrdinalIgnoreCase));
        return whitelistData
            .Select(e => new SymbolData
            {
                Symbol = $"{e.Symbol}.{e.Exchange}",
                Instance = e.InstanceConfigProfile
            }).ToList();
    }

    private async Task ProcessPublicTrades(HttpClient client, SymbolData data, long getDataTime,
        List<MarketDataSnapshot> snapshots)
    {
        var apiTradeTypeUrl = BuildApiUrl("ticks", data.Symbol ?? string.Empty, getDataTime, "trades");
        var response = await client.GetAsync(apiTradeTypeUrl);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Error: {StatusCode} {Symbol} from velexa API", response.StatusCode, data.Symbol);
            return;
        }

        _logger.LogDebug("Instance {Instance} Symbol: {Symbol} Public Trades data recovering ...", data.Instance,
            data.Symbol);
        var responseData = await response.Content.ReadAsStringAsync();
        var newSnapshots = ParseTradeResponse(responseData, data.Symbol ?? string.Empty);
        snapshots.AddRange(newSnapshots);
    }

    private async Task ProcessBidOffer(HttpClient client, SymbolData data, long getDataTime,
        List<MarketDataSnapshot> snapshots)
    {
        var apiQuoteTypeUrl = BuildApiUrl("ticks", data.Symbol ?? string.Empty, getDataTime);
        var response = await client.GetAsync(apiQuoteTypeUrl);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Error: {StatusCode} {Symbol} from velexa API", response.StatusCode, data.Symbol);
            return;
        }

        _logger.LogDebug("Instance: {Instance} Symbol: {Symbol} Bid/Offer data recovering ...", data.Instance,
            data.Symbol);
        var responseData = await response.Content.ReadAsStringAsync();
        var newSnapshot = ParseQuoteResponse(responseData, data.Symbol ?? string.Empty);

        if (newSnapshot != null)
            snapshots.Add(newSnapshot);
    }

    private string BuildApiUrl(string endpoint, string symbol, long from, string? type = null)
    {
        var velexaApiBaseUrl = _configuration.GetValue<string>(ConfigurationKeys.VelexaHttpApiBaseUrl);
        var velexaApiVersion = _configuration.GetValue<string>(ConfigurationKeys.VelexaApiVersion);
        var url = $"{velexaApiBaseUrl}{velexaApiVersion}/{endpoint}/{symbol}?from={from}";

        if (!string.IsNullOrEmpty(type)) url += $"&type={type}";

        return url;
    }

    private List<MarketDataSnapshot> ParseTradeResponse(string responseData, string symbol)
    {
        var snapshots = new List<MarketDataSnapshot>();
        var velexaApiVersion = _configuration.GetValue<string>(ConfigurationKeys.VelexaApiVersion);

        if (velexaApiVersion == "2.0")
        {
            var responseObj = JsonConvert.DeserializeObject<List<VelexaTicksTradesApi2Response>>(responseData);
            snapshots.AddRange(ParseTradeResponseV2(responseObj ?? [], symbol));
        }
        else if (velexaApiVersion == "3.0")
        {
            var responseObj = JsonConvert.DeserializeObject<List<VelexaTicksTradesApi3Response>>(responseData);
            snapshots.AddRange(ParseTradeResponseV3(responseObj ?? [], symbol));
        }

        return snapshots;
    }

    private List<MarketDataSnapshot> ParseTradeResponseV2(List<VelexaTicksTradesApi2Response> responseObj,
        string symbol)
    {
        if (responseObj.Count == 0) return [];

        return responseObj.OrderByDescending(e => e.Timestamp)
            .Take(50)
            .OrderBy(e => e.Timestamp)
            .Select(obj => new MarketDataSnapshot
            {
                Symbol = symbol,
                Entries =
                [
                    new MarketDataEntry
                    {
                        MdEntryType = "2",
                        MdEntrySize = decimal.TryParse(obj.Size, out var size) ? size : 0,
                        MdEntryPx = decimal.TryParse(obj.Value, out var value) ? value : 0,
                        MdEntryDate = DateTimeOffset.FromUnixTimeMilliseconds(obj.Timestamp).UtcDateTime.Date,
                        MdEntryTime = DateTimeOffset.FromUnixTimeMilliseconds(obj.Timestamp).UtcDateTime
                    }
                ]
            })
            .ToList();
    }

    private List<MarketDataSnapshot> ParseTradeResponseV3(List<VelexaTicksTradesApi3Response> responseObj,
        string symbol)
    {
        if (responseObj.Count == 0) return [];

        return responseObj.OrderByDescending(e => e.Timestamp)
            .Take(50)
            .OrderBy(e => e.Timestamp)
            .Select(obj => new MarketDataSnapshot
            {
                Symbol = symbol,
                Entries =
                [
                    new MarketDataEntry
                    {
                        MdEntryType = "2",
                        MdEntrySize = decimal.TryParse(obj.Size, out var size) ? size : 0,
                        MdEntryPx = decimal.TryParse(obj.Price, out var price) ? price : 0,
                        MdEntryDate = DateTimeOffset.FromUnixTimeMilliseconds(obj.Timestamp).UtcDateTime.Date,
                        MdEntryTime = DateTimeOffset.FromUnixTimeMilliseconds(obj.Timestamp).UtcDateTime
                    }
                ]
            })
            .ToList();
    }

    private MarketDataSnapshot? ParseQuoteResponse(string responseData, string symbol)
    {
        var velexaApiVersion = _configuration.GetValue<string>(ConfigurationKeys.VelexaApiVersion);

        if (velexaApiVersion == "2.0")
        {
            var responseObj = JsonConvert.DeserializeObject<List<VelexaTicksQuotesApi2Response>>(responseData);
            return ParseQuoteResponseV2(responseObj ?? [], symbol);
        }

        if (velexaApiVersion == "3.0")
        {
            var responseObj = JsonConvert.DeserializeObject<List<VelexaTicksQuotesApi3Response>>(responseData);
            return ParseQuoteResponseV3(responseObj ?? [], symbol);
        }

        return null;
    }

    private MarketDataSnapshot? ParseQuoteResponseV2(List<VelexaTicksQuotesApi2Response> responseObj, string symbol)
    {
        if (responseObj.Count == 0) return null;

        var obj = responseObj.MaxBy(e => e.Timestamp);
        var bidFirstOrDefault = obj?.Bid?.FirstOrDefault();
        var askFirstOrDefault = obj?.Ask?.FirstOrDefault();

        return new MarketDataSnapshot
        {
            Symbol = symbol,
            Entries =
            [
                CreateMarketDataEntry("0", bidFirstOrDefault?.Size ?? string.Empty,
                    bidFirstOrDefault?.Value ?? string.Empty, obj?.Timestamp),
                CreateMarketDataEntry("1", askFirstOrDefault?.Size ?? string.Empty,
                    askFirstOrDefault?.Value ?? string.Empty, obj?.Timestamp)
            ]
        };
    }

    private MarketDataSnapshot? ParseQuoteResponseV3(List<VelexaTicksQuotesApi3Response> responseObj, string symbol)
    {
        if (responseObj.Count == 0) return null;

        var obj = responseObj.MaxBy(e => e.Timestamp);
        var bidFirstOrDefault = obj?.Bid?.FirstOrDefault();
        var askFirstOrDefault = obj?.Ask?.FirstOrDefault();

        return new MarketDataSnapshot
        {
            Symbol = symbol,
            Entries =
            [
                CreateMarketDataEntry("0", bidFirstOrDefault?.Price ?? string.Empty,
                    bidFirstOrDefault?.Size ?? string.Empty, obj?.Timestamp),
                CreateMarketDataEntry("1", askFirstOrDefault?.Price ?? string.Empty,
                    askFirstOrDefault?.Size ?? string.Empty, obj?.Timestamp)
            ]
        };
    }

    private MarketDataEntry CreateMarketDataEntry(string entryType, string price, string size, long? timestamp)
    {
        return new MarketDataEntry
        {
            MdEntryType = entryType,
            MdEntryPx = decimal.TryParse(price, out var parsedPrice) ? parsedPrice : 0,
            MdEntrySize = decimal.TryParse(size, out var parsedSize) ? parsedSize : 0,
            MdEntryDate = DateTimeOffset.FromUnixTimeMilliseconds(timestamp ?? 0).UtcDateTime.Date,
            MdEntryTime = DateTimeOffset.FromUnixTimeMilliseconds(timestamp ?? 0).UtcDateTime
        };
    }

    private async Task PublishSnapshots(ICollection snapshots, string symbol)
    {
        if (snapshots.Count > 0)
        {
            var jsonResponse = JsonConvert.SerializeObject(snapshots);
            var dataRecoveryTopic = _configuration.GetValue<string>(ConfigurationKeys.KafkaDataRecoveryTopic);

            if (!string.IsNullOrEmpty(dataRecoveryTopic))
                await _kafkaPublisher.PublishAsync(
                    dataRecoveryTopic,
                    new StockMessage
                    {
                        MessageType = "MarketDataSnapshotFullRefresh",
                        Message = jsonResponse
                    }
                );

            _logger.LogDebug("{Symbol} has been recovered", symbol);
        }
    }
}