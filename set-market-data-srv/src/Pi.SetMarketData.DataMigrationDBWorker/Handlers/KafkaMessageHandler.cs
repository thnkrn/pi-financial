using Newtonsoft.Json;
using Confluent.Kafka;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.DataMigration;
using Pi.SetMarketData.Infrastructure.Interfaces.Kafka;
using Pi.SetMarketData.Domain.Models.Response.Sirius;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.SetMarketData.DataMigrationDBWorker.Handlers;

public class KafkaMessageHandler : IKafkaMessageHandler<string, string>
{
    private const string RealtimeTableName = "realtime_market_data";
    private readonly ILogger<KafkaMessageHandler> _logger;
    private readonly ITimescaleService<RealtimeMarketData> _realtimeMarketDataService;

    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="realtimeMarketDataService"></param>
    public KafkaMessageHandler
    (
        ILogger<KafkaMessageHandler> logger,
        ITimescaleService<RealtimeMarketData> realtimeMarketDataService
    )
    {
        _logger = logger;
        _realtimeMarketDataService = realtimeMarketDataService;
    }

    public async Task HandleAsync(ConsumeResult<string, string> consumeResult)
    {
        var message = consumeResult.Message.Value;
        _logger.LogDebug("Consume message: {Message}", message);

        var migrationData = JsonConvert.DeserializeObject<MigrationData<SiriusMarketIndicatorResponse>>(message);

        if (migrationData?.MigrationJob == null || migrationData.Response?.Response == null)
        {
            throw new InvalidOperationException("Invalid migration data structure");
        }

        var symbol = migrationData.MigrationJob.Symbol ?? throw new InvalidOperationException("Symbol is null");
        var venue = migrationData.Response.Response.Venue ?? throw new InvalidOperationException("Venue is null");

        var dataList = CreateRealtimeMarketDataList(migrationData.Response.Response.Candles, symbol, venue);

        const int batchSize = 500;
        for (int i = 0; i < dataList.Count; i += batchSize)
        {
            var batch = dataList.Skip(i).Take(batchSize).ToList();
            await _realtimeMarketDataService.BatchUpsertAsync(
                batch,
                RealtimeTableName,
                nameof(RealtimeMarketData.DateTime),
                nameof(RealtimeMarketData.Symbol),
                nameof(RealtimeMarketData.Venue));
        }
    }

    private List<RealtimeMarketData> CreateRealtimeMarketDataList(List<List<object>> candles, string symbol,
        string venue)
    {
        var dataList = new List<RealtimeMarketData>();

        foreach (var candle in candles)
        {
            var utcDateTime = DateTimeOffset.FromUnixTimeSeconds((long)candle[0]).UtcDateTime;

            dataList.AddRange
            (
            [
                CreateDataPoint(utcDateTime.AddSeconds(0), symbol, venue, candle[1], 0, 0), // Open
                CreateDataPoint(utcDateTime.AddSeconds(1), symbol, venue, candle[2], 0, 0), // High
                CreateDataPoint(utcDateTime.AddSeconds(2), symbol, venue, candle[3], 0, 0), // Low
                CreateDataPoint(utcDateTime.AddSeconds(3), symbol, venue, candle[4], candle[5], candle[6]) // Close
            ]);
        }

        return dataList;
    }

    private RealtimeMarketData CreateDataPoint(DateTime dateTime, string symbol, string venue, object price,
        object volume, object amount)
    {
        return new RealtimeMarketData
        {
            DateTime = dateTime,
            Symbol = symbol,
            Venue = venue,
            Price = Convert.ToDouble(price),
            Volume = (int)Math.Round(Convert.ToDouble(volume)),
            Amount = Convert.ToDouble(amount)
        };
    }
}