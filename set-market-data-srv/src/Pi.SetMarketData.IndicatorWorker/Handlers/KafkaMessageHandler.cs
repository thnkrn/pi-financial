using Confluent.Kafka;
using Newtonsoft.Json;
using Skender.Stock.Indicators;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Indicator;
using Pi.SetMarketData.Infrastructure.Interfaces.Kafka;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Domain.ConstantConfigurations;

namespace Pi.SetMarketData.IndicatorWorker.Handlers;

public class KafkaMessageHandler : IKafkaMessageHandler<string, string>
{
    private const string _prefix = "timescaledb.public.candle_";
    private readonly string _failedTopic;
    private readonly ILogger<KafkaMessageHandler> _logger;
    private readonly IKafkaPublisher<string, string> _publisher;
    private readonly ITimescaleService<TechnicalIndicators> _technicalIndicatorsService;
    private readonly ITimescaleService<CandleData> _candleDataService;
    private readonly int _indicatorMigrationBatchSize;

    public KafkaMessageHandler
    (
        IConfiguration configuration,
        ILogger<KafkaMessageHandler> logger,
        IKafkaPublisher<string, string> publisher,
        ITimescaleService<TechnicalIndicators> technicalIndicatorsService,
        ITimescaleService<CandleData> candleDataService
    )
    {
        _logger = logger;
        _publisher = publisher;
        _technicalIndicatorsService = technicalIndicatorsService;
        _candleDataService = candleDataService;
        _failedTopic = ConfigurationHelper.GetTopicList(configuration, ConfigurationKeys.KafkaFailedTopic).FirstOrDefault()
            ?? throw new InvalidOperationException("KafkaFailedTopic is not configured.");
        _indicatorMigrationBatchSize = configuration.GetValue<int>(ConfigurationKeys.IndicatorMigrationBatchSize, 5);
    }

    public async Task HandleAsync(ConsumeResult<string, string> consumeResult)
    {
        var message = consumeResult.Message.Value;
        _logger.LogDebug("Topic: {Topic}", consumeResult.Topic);
        _logger.LogDebug("Message: {Message}", message);

        var validMessage = false;
        // Try deserialize as CandleEventMessage first
        var candleEventMessage = JsonConvert.DeserializeObject<CandleEventMessage>(message);
        if (candleEventMessage?.After != null)
        {
            validMessage = true;
            await ProcessCandleEventMessage(candleEventMessage, consumeResult.Topic);
        }

        // If not CandleEventMessage, try IndicatorMigrationMessage
        var migrationMessage = JsonConvert.DeserializeObject<IndicatorMigrationMessage>(message);
        if (migrationMessage != null && migrationMessage.Timeframe != null)
        {
            validMessage = true;
            await ProcessIndicatorMigrationMessage(migrationMessage);
        }

        if (!validMessage)
        {
            // If neither message type is valid, publish to failed topic
            await _publisher.PublishAsync(_failedTopic, new Message<string, string> { Value = message });
            _logger.LogError("Unable to deserialize message as either CandleEventMessage or IndicatorMigrationMessage: {Message}", message);
        }
    }

    public async Task<TechnicalIndicators> CalculateIndicators(
        string timeframe,
        string symbol,
        string venue,
        DateTime lookbackUpperBoundDate
    )
    {
        // We need at least 150 points for MACD
        // https://dotnet.stockindicators.dev/indicators/Macd/
        var lookbackSteps = 150;
        var lookbackLowerBoundDate = GetLookbackLowerBoundDate(timeframe, lookbackUpperBoundDate, lookbackSteps);
        var candleData = await _candleDataService.GetCandlesAsync(
            timeframe,
            symbol,
            venue,
            lookbackSteps,
            lookbackLowerBoundDate,
            lookbackUpperBoundDate
        );

        /* ------ SMA ----- */
        var sma10 = candleData.GetSma(10).TakeLast(1).ToList();
        var sma25 = candleData.GetSma(25).TakeLast(1).ToList();

        /* ------ EMA ----- */
        var ema10 = candleData.GetEma(10).TakeLast(1).ToList();
        var ema25 = candleData.GetEma(25).TakeLast(1).ToList();

        /* ------ MACD ----- */
        var macd = candleData.GetMacd(12, 26, 9).TakeLast(1).ToList();

        /* ------ RSI ----- */
        var rsi = candleData.GetRsi(14).TakeLast(1).ToList();

        decimal gain = 0;
        decimal loss = 0;
        decimal diff = 0;
        if (candleData.Count > 2)
        {
            diff = candleData[candleData.Count - 1].Close - candleData[candleData.Count - 2].Close;
        }
        if (diff > 0)
        {
            gain = diff;
        }
        else
        {
            loss = diff;
        }

        /* ------ BOLL ----- */
        var boll = candleData.GetBollingerBands(20).TakeLast(1).ToList();

        /* ------ KDJ ----- */
        var kdj = candleData.GetStoch(14, 3, 1, 3, 2, MaType.SMA).TakeLast(1).ToList();

        var indicators = new TechnicalIndicators
        {
            DateTime = lookbackUpperBoundDate,
            Symbol = symbol,
            Venue = venue,
            Sma10 = sma10.FirstOrDefault()?.Sma ?? 0,
            Sma25 = sma25.FirstOrDefault()?.Sma ?? 0,
            Ema10 = ema10.FirstOrDefault()?.Ema ?? 0,
            Ema25 = ema25.FirstOrDefault()?.Ema ?? 0,
            MacdEma12 = macd.FirstOrDefault()?.FastEma ?? 0,
            MacdEma26 = macd.FirstOrDefault()?.SlowEma ?? 0,
            MacdMacdDiff = macd.FirstOrDefault()?.Macd ?? 0,
            MacdSignalDea = macd.FirstOrDefault()?.Signal ?? 0,
            MacdOsc = (macd.FirstOrDefault()?.Macd ?? 0) - (macd.FirstOrDefault()?.Signal ?? 0),
            RsiRsi = rsi.FirstOrDefault()?.Rsi ?? 0,
            RsiGainSmmaUp = (double)gain,
            RsiLossSmmaDown = (double)loss,
            BollUpper = boll.FirstOrDefault()?.UpperBand ?? 0,
            BollMedium = boll.FirstOrDefault()?.Sma ?? 0, // Middle band = 20-day SMA
            BollLower = boll.FirstOrDefault()?.LowerBand ?? 0,
            KdjK = kdj.FirstOrDefault()?.K ?? 0,
            KdjD = kdj.FirstOrDefault()?.D ?? 0,
            KdjJ = kdj.FirstOrDefault()?.J ?? 0
        };

        return indicators;
    }

    private static string RemovePrefix(string topic)
    {
        topic = topic
            .Replace(_prefix, string.Empty)
            .Replace("_", string.Empty);

        topic = topic switch
        {
            "1hour" => CandleType.candle60Min,
            "2hour" => CandleType.candle120Min,
            "4hour" => CandleType.candle240Min,
            _ => topic
        };
        return topic;
    }

    public async Task ProcessCandleEventMessage(CandleEventMessage candleEventMessage, string consumeResultTopic)
    {
        var symbol = candleEventMessage.After?.Symbol;
        var venue = candleEventMessage.After?.Venue;
        var candleDateTime = candleEventMessage.After?.Bucket;
        if (symbol == null || venue == null || candleDateTime == null) return;

        var timeframe = RemovePrefix(consumeResultTopic);
        var indicators = await CalculateIndicators(timeframe, symbol, venue, candleDateTime.Value.UtcDateTime);
        await _technicalIndicatorsService.UpsertTechnicalIndicators(timeframe, indicators);
    }

    public async Task ProcessIndicatorMigrationMessage(IndicatorMigrationMessage migrationMessage)
    {
        var from = migrationMessage.DateTimeFrom;
        var to = migrationMessage.DateTimeTo;
        var timeframe = migrationMessage.Timeframe;
        _logger.LogInformation("******** From {From} to {To}, timeframe {Timeframe}", from, to, timeframe);

        var minutesInterval = GetTimeframeMinutes(timeframe);
        var batchSize = _indicatorMigrationBatchSize;
        var tasks = new List<Task<TechnicalIndicators>>();

        for (var calculateDate = from; calculateDate <= to; calculateDate = calculateDate.AddMinutes(minutesInterval))
        {
            tasks.Add(CalculateIndicators(RemovePrefix(timeframe), migrationMessage.Symbol, migrationMessage.Venue, calculateDate.UtcDateTime));

            if (tasks.Count >= batchSize)
            {
                _logger.LogInformation("{Symbol}({TimeframeInt}), now processing {CalculateDate} (From {From} to {To}, batch size {BatchSize})", migrationMessage.Symbol, timeframe, calculateDate.UtcDateTime, from, to, batchSize);
                await Task.WhenAll(tasks);
                var indicators = new List<TechnicalIndicators>();
                foreach (var t in tasks)
                {
                    indicators.Add(t.Result);
                }
                await _technicalIndicatorsService.UpsertBatchTechnicalIndicators(RemovePrefix(timeframe), indicators);
                tasks.Clear();
            }
        }

        // Process any remaining tasks
        if (tasks.Any())
        {
            var indicators = new List<TechnicalIndicators>();
            foreach (var t in tasks)
            {
                indicators.Add(t.Result);
            }
            await _technicalIndicatorsService.UpsertBatchTechnicalIndicators(RemovePrefix(timeframe), indicators);
        }
    }

    private static DateTime GetLookbackLowerBoundDate(string timeframe, DateTime lookbackUpperBoundDate, int steps)
    {
        int minutesInterval = GetTimeframeMinutes(timeframe);
        var lookbackLowerBoundDate = lookbackUpperBoundDate.AddMinutes(-minutesInterval * steps);
        switch (timeframe)
        {
            // safe guard for holidays
            // we need to add more days in case of holidays
            case "1min": // 60 * 24 * 3 = 4320
            case "2min": // 30 * 24 * 3 = 2160
            case "5min": // 12 * 24 * 3 = 864
                lookbackLowerBoundDate = lookbackLowerBoundDate.AddDays(-3);
                break;
            case "15min": // 4 * 24 * 7 = 672
            case "30min": // 2 * 24 * 7 = 336
                lookbackLowerBoundDate = lookbackLowerBoundDate.AddDays(-7);
                break;
            case "1hour" or "candle60min" or "60min": // 1 * 24 * 14 = 336
                lookbackLowerBoundDate = lookbackLowerBoundDate.AddDays(-14);
                break;
            case "2hour" or "candle120min" or "120min": // 12 * 28 = 360
                lookbackLowerBoundDate = lookbackLowerBoundDate.AddDays(-30);
                break;
            case "4hour" or "candle240min" or "240min": // 6 * 50 = 300
                lookbackLowerBoundDate = lookbackLowerBoundDate.AddDays(-50);
                break;
            case "1day":
                lookbackLowerBoundDate = lookbackLowerBoundDate.AddDays(-300);
                break;
            case "1week": // 4 * 80 = 320
                lookbackLowerBoundDate = lookbackLowerBoundDate.AddMonths(-80);
                break;
            case "1month":
                lookbackLowerBoundDate = lookbackLowerBoundDate.AddMonths(-300);
                break;
            default:
                throw new ArgumentException($"Invalid timeframe: {timeframe}", nameof(timeframe));
        }
        return lookbackLowerBoundDate;
    }

    private static int GetTimeframeMinutes(string timeframe)
    {
        return timeframe switch
        {
            "1min" => 1,
            "2min" => 2,
            "5min" => 5,
            "15min" => 15,
            "30min" => 30,
            "1hour" or "candle60min" or "60min" => 60,
            "2hour" or "candle120min" or "120min" => 120,
            "4hour" or "candle240min" or "240min" => 240,
            "1day" => 1440,
            "1week" => 10080,
            "1month" => 43200,
            _ => throw new ArgumentException($"Invalid timeframe: {timeframe}", nameof(timeframe))
        };
    }
}
