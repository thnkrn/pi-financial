using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.IndicatorWorker.Handlers;
using Pi.SetMarketData.Infrastructure.Interfaces.Kafka;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.SetMarketData.IndicatorWorker.Tests.Handlers;

public class KafkaMessageHandlerTest
{
    private readonly KafkaMessageHandler _kafkaMessageHandler;
    private readonly Mock<ITimescaleService<CandleData>> _mockCandleDataService;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<ILogger<KafkaMessageHandler>> _mockLogger;
    private readonly Mock<IKafkaPublisher<string, string>> _mockPublisher;
    private readonly Mock<ITimescaleService<TechnicalIndicators>> _mockTechnicalIndicatorsService;

    public KafkaMessageHandlerTest()
    {
        _mockConfig = new Mock<IConfiguration>();
        _mockPublisher = new Mock<IKafkaPublisher<string, string>>();
        _mockLogger = new Mock<ILogger<KafkaMessageHandler>>();
        _mockTechnicalIndicatorsService = new Mock<ITimescaleService<TechnicalIndicators>>();
        _mockCandleDataService = new Mock<ITimescaleService<CandleData>>();

        _kafkaMessageHandler = new KafkaMessageHandler(_mockConfig.Object,
            _mockLogger.Object,
            _mockPublisher.Object,
            _mockTechnicalIndicatorsService.Object,
            _mockCandleDataService.Object);
    }

    [Fact]
    public async Task CalculateIndicators_ShouldCalculateAndUpsertIndicators()
    {
        // Arrange
        var timeframe = "1_min";
        var symbol = "APPL";
        var venue = "NASDAQ";
        var lookbackStartFrom = DateTime.UtcNow;

        var candleData = GenerateMockCandleData(symbol, venue);
        _mockCandleDataService.Setup(s => s.GetCandlesAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>(), null, It.IsAny<DateTime>()))
            .ReturnsAsync(candleData);

        // Act
        await _kafkaMessageHandler.CalculateIndicators(timeframe, symbol, venue, lookbackStartFrom);

        // Assert
        _mockTechnicalIndicatorsService.Verify(s => s.UpsertTechnicalIndicators(
            It.IsAny<string>(), It.IsAny<TechnicalIndicators>()), Times.Once);
    }

    private List<CandleData> GenerateMockCandleData(string symbol, string venue)
    {
        return Enumerable.Range(0, 300).Select(i => new CandleData
        {
            Date = DateTime.UtcNow.AddHours(-i),
            Symbol = symbol,
            Venue = venue,
            OpenDouble = 100 + i,
            HighDouble = 105 + i,
            LowDouble = 95 + i,
            CloseDouble = 102 + i,
            VolumeDouble = 1000 + i,
            AmountDouble = (102 + i) * (1000 + i) // Close * Volume as a simple calculation for Amount
        }).ToList();
    }
}