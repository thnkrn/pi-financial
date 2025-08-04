using System.Linq.Expressions;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.SetMarketData.Application.Interfaces.ItchMapper;
using Pi.SetMarketData.Application.Models.ItchMessageWrapper;
using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.DataProcessingService.Handlers;
using Pi.SetMarketData.DataProcessingService.Interface;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;
using Pi.SetMarketData.Infrastructure.Models.Kafka;
using Timestamp = Pi.SetMarketData.Application.Services.Types.ItchParser.Timestamp;

namespace Pi.SetMarketData.DataProcessingService.Tests.Handlers;

public class KafkaMessageHandlerTests
{
    private readonly KafkaMessageHandler _handler;
    private readonly Mock<IRedisV2Publisher> _mockCacheService;
    private readonly Mock<ICacheServiceHelper> _mockCacheServiceHelper;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IMongoService<CorporateAction>> _mockCorporateActionService;
    private readonly Mock<IMongoService<InstrumentDetail>> _mockInstrumentDetailService;
    private readonly Mock<IMongoService<Instrument>> _mockInstrumentService;
    private readonly Mock<IItchMapperService> _mockItchMapperService;
    private readonly Mock<ILogger<KafkaMessageHandler>> _mockLogger;
    private readonly Mock<IMongoService<MorningStarFlag>> _mockMorningStarFlagService;
    private readonly Mock<IMongoService<OrderBook>> _mockOrderBookService;
    private readonly Mock<IMongoService<PriceInfo>> _mockPriceInfoService;
    private readonly Mock<ITimescaleService<RealtimeMarketData>> _mockRealtimeMarketDataService;
    private readonly Mock<IServiceScopeFactory> _mockServiceScopeFactory;
    private readonly Mock<IMongoService<TradingSign>> _mockTradingSignService;
    private readonly Mock<IMongoService<WhiteList>> _mockWhiteListService;

    public KafkaMessageHandlerTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockItchMapperService = new Mock<IItchMapperService>();
        _mockRealtimeMarketDataService = new Mock<ITimescaleService<RealtimeMarketData>>();
        _mockCacheServiceHelper = new Mock<ICacheServiceHelper>();
        _mockLogger = new Mock<ILogger<KafkaMessageHandler>>();
        _mockPriceInfoService = new Mock<IMongoService<PriceInfo>>();
        _mockOrderBookService = new Mock<IMongoService<OrderBook>>();
        _mockInstrumentService = new Mock<IMongoService<Instrument>>();
        _mockInstrumentDetailService = new Mock<IMongoService<InstrumentDetail>>();
        _mockWhiteListService = new Mock<IMongoService<WhiteList>>();
        _mockCorporateActionService = new Mock<IMongoService<CorporateAction>>();
        _mockMorningStarFlagService = new Mock<IMongoService<MorningStarFlag>>();
        _mockTradingSignService = new Mock<IMongoService<TradingSign>>();
        _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        _mockCacheService = new Mock<IRedisV2Publisher>();

        var dependencies = new KafkaMessageHandlerDependencies(
            _mockInstrumentDetailService.Object,
            _mockInstrumentService.Object,
            _mockOrderBookService.Object,
            _mockPriceInfoService.Object,
            _mockWhiteListService.Object,
            _mockCorporateActionService.Object,
            _mockTradingSignService.Object
        );

        var moreDependencies = new KafkaMessageHandlerMoreDependencies(
            _mockCacheService.Object,
            _mockCacheServiceHelper.Object,
            _mockMorningStarFlagService.Object,
            _mockServiceScopeFactory.Object
        );

        _handler = new KafkaMessageHandler(
            _mockConfiguration.Object,
            _mockItchMapperService.Object,
            dependencies,
            moreDependencies,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task HandleAsync_EmptyMessage_LogsWarning()
    {
        // Arrange
        var emptyMessage = "";
        var consumeResult = new Message<string, string>
        {
            Value = emptyMessage
        };

        // Act
        await _handler.HandleAsync(consumeResult);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Received empty message")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_InvalidJsonMessage_LogsWarning()
    {
        // Arrange
        const string invalidJsonMessage = "This is not a valid JSON";
        var consumeResult = new Message<string, string>
        {
            Value = invalidJsonMessage
        };

        // Act
        await _handler.HandleAsync(consumeResult);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o.ToString().Contains("The message cannot be deserialized because it is invalid")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ValidTradeTickerMessage_ProcessesCorrectly()
    {
        // Arrange
        var tradeTickerMessage = new StockMessage
        {
            MessageType = "i",
            Message = JsonConvert.SerializeObject(new TradeTickerMessageWrapper
            {
                Action = new Numeric { Value = 1 },
                OrderbookId = new Numeric { Value = 123 },
                Price = new Price64 { Value = 100 },
                Quantity = new Numeric { Value = 100 },
                DealDateTime = new Timestamp(DateTimeOffset.Now.ToUnixTimeSeconds())
            })
        };

        var message = JsonConvert.SerializeObject(tradeTickerMessage);
        var consumeResult = new Message<string, string>
        {
            Value = message
        };

        _mockCacheServiceHelper.Setup(x => x.GetPriceInfoByOrderBookId(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(new PriceInfo { Symbol = "TEST" });

        _mockCacheServiceHelper.Setup(x => x.GetVenueBySymbol(It.IsAny<string>()))
            .ReturnsAsync("TEST_VENUE");

        _mockInstrumentDetailService
            .Setup(x => x.GetByFilterAsync(It.IsAny<Expression<Func<InstrumentDetail, bool>>>()))
            .ReturnsAsync(new InstrumentDetail { DecimalsInPrice = 2 });

        _mockRealtimeMarketDataService
            .Setup(x => x.UpsertAsync(It.IsAny<RealtimeMarketData>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(consumeResult);

        // Assert
        _mockRealtimeMarketDataService.Verify(
            x => x.UpsertAsync(
                It.IsAny<RealtimeMarketData>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Once);
    }
}