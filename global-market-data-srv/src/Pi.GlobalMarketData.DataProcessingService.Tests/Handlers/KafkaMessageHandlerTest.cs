using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.GlobalMarketData.DataProcessingService.Handlers;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.GlobalMarketData.DataProcessingService.Tests.Handlers;

public class KafkaMessageHandlerTest
{
    private readonly KafkaMessageHandler _handler;
    private readonly Mock<IMongoService<WhiteList>> _mockWhiteListService;

    private readonly Mock<ITimescaleService<RealtimeMarketData>> _mockTimeScaleService;

    public KafkaMessageHandlerTest()
    {
        var mockLogger = new Mock<ILogger<KafkaMessageHandler>>();
        _mockWhiteListService = new Mock<IMongoService<WhiteList>>();
        _mockTimeScaleService = new Mock<ITimescaleService<RealtimeMarketData>>();

        _handler = new KafkaMessageHandler(
            mockLogger.Object,
            _mockWhiteListService.Object,
            null,
            null,
            null
        );
    }

    [Fact]
    public async Task Call_HandleAsync_ShouldRunCorrectly()
    {
        // Arrange
        var subscribeKafkaMessage = new
        {
            Message =
                "{\r\n  \"Symbol\": \"AAPL.NASDAQ\",\r\n  \"MDReqID\": \"reqID375\",\r\n  \"Entries\": [\r\n    {\r\n      \"MDEntryType\": \"2\",\r\n      \"MDEntryPx\": 226.55,\r\n      \"MDEntrySize\": 100.0,\r\n      \"MDEntryDate\": \"2024-08-20T00:00:00\",\r\n      \"MDEntryTime\": \"2024-08-21T23:58:16.939\"\r\n    }\r\n  ]\r\n}",
            MessageType = "MarketDataSnapshotFullRefresh"
        };

        var consumeResult = new Message<string, string>()
        {
            Key = DateTime.UtcNow.Ticks.ToString(),
            Value = JsonSerializer.Serialize(subscribeKafkaMessage)
        };


        // Act
        await _handler.HandleAsync(consumeResult);

        // Assert
        _mockWhiteListService.Verify();
    }
}