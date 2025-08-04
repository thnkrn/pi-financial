using Confluent.Kafka;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.SetMarketData.DataMigrationDBWorker.Handlers;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.DataMigration;
using Pi.SetMarketData.Domain.Models.Response.Sirius;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.SetMarketData.DataMigrationDBWorker.Tests.Services;

public class DBWorkerServiceTest
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ITimescaleService<RealtimeMarketData>> _mockMarketDataService;
    private readonly Mock<ILogger<KafkaMessageHandler>> _mockLogger;
    private readonly KafkaMessageHandler _kafkaMessageHandler;
    public DBWorkerServiceTest()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockMarketDataService = new Mock<ITimescaleService<RealtimeMarketData>>();
        _mockLogger = new Mock<ILogger<KafkaMessageHandler>>();

        SetupMockConfiguration();

        _kafkaMessageHandler = new KafkaMessageHandler(
            _mockLogger.Object,
            _mockMarketDataService.Object
        );

    }

    private void SetupMockConfiguration()
    {
        var mockConfigSection = new Mock<IConfigurationSection>();
        mockConfigSection.Setup(x => x.Value).Returns("test_server");
        _mockConfiguration
            .Setup(x => x.GetSection(ConfigurationKeys.KafkaBootstrapServers))
            .Returns(mockConfigSection.Object);

        mockConfigSection.Setup(x => x.Value).Returns("test_group");
        _mockConfiguration
            .Setup(x => x.GetSection(ConfigurationKeys.KafkaConsumerGroupId))
            .Returns(mockConfigSection.Object);

        mockConfigSection.Setup(x => x.Value).Returns("test_topic");
        _mockConfiguration
            .Setup(x => x.GetSection(ConfigurationKeys.KafkaTopic))
            .Returns(mockConfigSection.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidMessage_CallsUpsertAsync()
    {
        // Arrange
        var realtimeTableName = "realtime_market_data";
        var migrationData = new MigrationData<SiriusMarketIndicatorResponse>
        {
            MigrationJob = new MigrationJob { Symbol = "BTCUSD" },
            Response = new SiriusMarketIndicatorResponse
            {
                Response = new MarketIndicator
                {
                    Venue = "TestVenue",
                    Candles = new List<List<object>>
                    {
                        new List<object> { 1631145600, 50000.0, 51000.0, 49000.0, 50500.0, 100, 5050000.0 }
                    }
                }
            }
        };

        var messageValue = JsonConvert.SerializeObject(migrationData);

        var consumeResult = new ConsumeResult<string, string>
        {
            Message = new Message<string, string>
            {
                Value = messageValue
            }
        };

        // Act
        await _kafkaMessageHandler.HandleAsync(consumeResult);


        // Assert
        _mockMarketDataService.Verify(x => x.UpsertAsync(
            It.IsAny<RealtimeMarketData>(),
            It.Is<string>(s => s == realtimeTableName),
            It.Is<string>(s => s == nameof(RealtimeMarketData.DateTime)),
            It.Is<string>(s => s == nameof(RealtimeMarketData.Symbol)),
            It.Is<string>(s => s == nameof(RealtimeMarketData.Venue))
        ), Times.Exactly(4));
    }

    [Fact]
    public void CreateRealtimeMarketDataList_ValidInput_ReturnsCorrectData()
    {
        // Arrange
        var candles = new List<List<object>>
        {
            new() { 1631145600L, 50000.0, 51000.0, 49000.0, 50500.0, 100, 5050000.0 }
        };
        var symbol = "BTCUSD";
        var venue = "TestVenue";

        // Act
        var method = _kafkaMessageHandler.GetType().GetMethod("CreateRealtimeMarketDataList", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(method);

        var result = method.Invoke(_kafkaMessageHandler, [candles, symbol, venue]) as List<RealtimeMarketData>;
        Assert.NotNull(result);

        // Assert
        Assert.Equal(4, result.Count); // One candle should produce 4 data points

        // Check the first data point (Open)
        Assert.Equal(new DateTime(2021, 9, 9, 0, 0, 0, DateTimeKind.Utc), result[0].DateTime);
        Assert.Equal(symbol, result[0].Symbol);
        Assert.Equal(venue, result[0].Venue);
        Assert.Equal(50000.0, result[0].Price);
        Assert.Equal(0, result[0].Volume);
        Assert.Equal(0.0, result[0].Amount);

        // Check the second data point (High)
        Assert.Equal(new DateTime(2021, 9, 9, 0, 0, 1, DateTimeKind.Utc), result[1].DateTime);
        Assert.Equal(51000.0, result[1].Price);

        // Check the third data point (Low)
        Assert.Equal(new DateTime(2021, 9, 9, 0, 0, 2, DateTimeKind.Utc), result[2].DateTime);
        Assert.Equal(49000.0, result[2].Price);

        // Check the fourth data point (Close)
        Assert.Equal(new DateTime(2021, 9, 9, 0, 0, 3, DateTimeKind.Utc), result[3].DateTime);
        Assert.Equal(50500.0, result[3].Price);
        Assert.Equal(100, result[3].Volume);
        Assert.Equal(5050000.0, result[3].Amount);
    }
}