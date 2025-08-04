using Newtonsoft.Json;
using Confluent.Kafka;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.DataMigration;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;
using Pi.GlobalMarketData.Domain.Models.Response.Sirius;
using Pi.GlobalMarketData.Domain.Models.Response.Velexa;
using Pi.GlobalMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.GlobalMarketData.DataMigrationDBWorker.Handlers;

public class KafkaMessageHandler : IKafkaMessageHandler<string, string>
{
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

    private List<RealtimeMarketData> CreateRealtimeMarketDataList(List<VelexaOhlcResponse> candles, string symbol, string venue)
    {
        var dataList = new List<RealtimeMarketData>();

        foreach (var candle in candles)
        {
            var utcDateTime = DateTimeOffset.FromUnixTimeMilliseconds(candle.Timestamp).UtcDateTime;

            dataList.AddRange
            (
                [
                    CreateDataPoint(utcDateTime.AddSeconds(0), symbol, venue, candle.Open, 0, 0), // Open
                    CreateDataPoint(utcDateTime.AddSeconds(1), symbol, venue, candle.High, 0, 0), // High
                    CreateDataPoint(utcDateTime.AddSeconds(2), symbol, venue, candle.Low, 0, 0), // Low
                    CreateDataPoint(utcDateTime.AddSeconds(3), symbol, venue, candle.Close, candle.Volume, 0) // Close
            ]);
        }

        return dataList;
    }

    private RealtimeMarketData CreateDataPoint(DateTime dateTime, string symbol, string venue, object price, object volume, object amount)
    {
        return new RealtimeMarketData
        {
            DateTime = dateTime,
            Symbol = symbol,
            Venue = venue,
            Price = Convert.ToDouble(price),
            Volume = Convert.ToInt32(volume),
            Amount = Convert.ToDouble(amount)
        };
    }

    public async Task HandleAsync(ConsumeResult<string, string> consumeResult)
    {
        _logger.LogInformation("Consume message: {Message}", consumeResult);

        var migrationData = JsonConvert.DeserializeObject<MigrationData<List<VelexaOhlcResponse>>>(consumeResult.Message.Value);

        if (migrationData?.MigrationJob == null || migrationData.Response == null)
        {
            throw new InvalidOperationException("Invalid migration data structure");
        }

        var symbol = migrationData.MigrationJob.Symbol ?? throw new InvalidOperationException("Symbol is null");
        var venue = migrationData.MigrationJob.Venue ?? throw new InvalidOperationException("Venue is null");

        var dataList = CreateRealtimeMarketDataList(migrationData.Response, symbol, venue);

        foreach (var data in dataList)
        {
            await _realtimeMarketDataService.UpsertAsync
            (
                data,
                nameof(RealtimeMarketData.DateTime),
                nameof(RealtimeMarketData.Symbol),
                nameof(RealtimeMarketData.Venue)
            );
        }
    }
}