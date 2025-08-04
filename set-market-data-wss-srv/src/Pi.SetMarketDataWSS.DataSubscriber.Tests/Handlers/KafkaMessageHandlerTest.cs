using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.SetMarketDataWSS.Application.Interfaces.ItchHousekeeper;
using Pi.SetMarketDataWSS.Application.Interfaces.ItchMapper;
using Pi.SetMarketDataWSS.Application.Interfaces.StreamingResponseBuilder;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.DataSubscriber.BidOfferMapper;
using Pi.SetMarketDataWSS.DataSubscriber.Handlers;
using Pi.SetMarketDataWSS.DataSubscriber.Services;
using Pi.SetMarketDataWSS.Domain.Models.Kafka;
using Pi.SetMarketDataWSS.Domain.Models.Redis;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Redis;

namespace Pi.SetMarketDataWSS.DataSubscriber.Tests.Handlers;

public class KafkaMessageHandlerTest
{
    private readonly Mock<IConfiguration> _configuration;
    private readonly Mock<ICacheService> _distributedCache;
    private readonly Mock<IItchHousekeeperService> _itchHousekeeperService;
    private readonly Mock<IItchMapperService> _itchMapperService;
    private readonly Mock<ILogger<KafkaMessageHandler>> _logger;
    private readonly Mock<IMarketStreamingResponseBuilder> _marketStreamingResponseBuilder;

    private readonly Mock<IRedisV2Publisher> _redisPublisher;
    private readonly KafkaMessageHandler kafkaMessageHandler;
    private readonly Mock<IBidOfferService> _bidOfferService;
    private readonly Mock<BackgroundTaskQueue> _backgroundTaskQueue;

    public KafkaMessageHandlerTest()
    {
        _redisPublisher = new Mock<IRedisV2Publisher>();
        _itchMapperService = new Mock<IItchMapperService>();
        _configuration = new Mock<IConfiguration>();
        _distributedCache = new Mock<ICacheService>();
        _marketStreamingResponseBuilder = new Mock<IMarketStreamingResponseBuilder>();
        _logger = new Mock<ILogger<KafkaMessageHandler>>();
        _itchHousekeeperService = new Mock<IItchHousekeeperService>();
        _bidOfferService = new Mock<IBidOfferService>();
        _backgroundTaskQueue = new Mock<BackgroundTaskQueue>();
        var mockConfigSection = new Mock<IConfigurationSection>();

        _configuration
            .Setup(x => x.GetSection(It.IsAny<string>()))
            .Returns(mockConfigSection.Object);

        var dependencies = new KafkaMessageHandlerDependencies
        (
            _marketStreamingResponseBuilder.Object,
            _itchMapperService.Object,
            _itchHousekeeperService.Object,
            _bidOfferService.Object,
            _backgroundTaskQueue.Object
        );

        kafkaMessageHandler = new KafkaMessageHandler(
            _redisPublisher.Object,
            dependencies,
            _configuration.Object,
            _logger.Object
        );
    }

    [Fact]
    public async Task Msg_J_Run_HandleIndexMessageHandler()
    {
        // Arrange
        var msg =
            "{\"Message\":\"{\\\"Nanos\\\":{\\\"Value\\\":85160151},\\\"OrderbookId\\\":{\\\"Value\\\":125580},\\\"Value\\\":{\\\"Value\\\":5899,\\\"NumberOfDecimals\\\":0},\\\"HighValue\\\":{\\\"Value\\\":5913,\\\"NumberOfDecimals\\\":0},\\\"LowValue\\\":{\\\"Value\\\":5870,\\\"NumberOfDecimals\\\":0},\\\"OpenValue\\\":{\\\"Value\\\":5894,\\\"NumberOfDecimals\\\":0},\\\"TradedVolume\\\":{\\\"Value\\\":106761000},\\\"TradedValue\\\":{\\\"Value\\\":10079607800,\\\"NumberOfDecimals\\\":0},\\\"Change\\\":{\\\"Value\\\":9,\\\"NumberOfDecimals\\\":0},\\\"ChangePercent\\\":{\\\"Value\\\":15,\\\"NumberOfDecimals\\\":0},\\\"PreviousClose\\\":{\\\"Value\\\":5890,\\\"NumberOfDecimals\\\":0},\\\"Close\\\":{\\\"Value\\\":0,\\\"NumberOfDecimals\\\":0},\\\"Timestamp\\\":{},\\\"MsgType\\\":\\\"J\\\",\\\"Metadata\\\":null}\",\"MessageType\":\"J\"}";

        var kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = msg,
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        // Act
        await kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        _itchMapperService.Verify(
            x => x.MapToDataCache(It.IsAny<ItchMessage>(), It.IsAny<Dictionary<string, string>>()),
            Times.AtLeastOnce
        );
    }

    [Fact]
    public async Task All_Msg_Run_HandleMessageHandler()
    {
        const string msgR =
            "{\"Message\":\"{\\\"Nanos\\\":{\\\"Value\\\":111442352},\\\"OrderBookID\\\":{\\\"Value\\\":65804},\\\"Symbol\\\":{\\\"Value\\\":\\\"CPALL\\\"},\\\"LongName\\\":{\\\"Value\\\":\\\"CPALL_CP ALL\\\"},\\\"ISIN\\\":{\\\"Value\\\":\\\"\\\"},\\\"FinancialProduct\\\":{\\\"Value\\\":\\\"CS\\\"},\\\"TradingCurrency\\\":{\\\"Value\\\":\\\"THB\\\"},\\\"DecimalsInPrice\\\":{\\\"Value\\\":5},\\\"DecimalsInNominalValue\\\":{\\\"Value\\\":0},\\\"RoundLotSize\\\":{\\\"Value\\\":100},\\\"NominalValue\\\":{\\\"Value\\\":0},\\\"NumberOfLegs\\\":{\\\"Value\\\":0},\\\"UnderlyingName\\\":{\\\"Value\\\":\\\"CPALL\\\"},\\\"Underlying\\\":{\\\"Value\\\":268},\\\"UnderlyingOrderBookID\\\":{\\\"Value\\\":0},\\\"StrikePrice\\\":{\\\"Value\\\":0,\\\"NumberOfDecimals\\\":0},\\\"ExpirationDate\\\":{\\\"Value\\\":\\\"0001-01-01T00:00:00\\\"},\\\"DecimalsInStrikePrice\\\":{\\\"Value\\\":0},\\\"OptionType\\\":{\\\"Value\\\":0},\\\"ExchangeCode\\\":{\\\"Value\\\":35},\\\"MarketCode\\\":{\\\"Value\\\":11},\\\"PriceQuotationFactor\\\":{\\\"Value\\\":1,\\\"NumberOfDecimals\\\":0},\\\"CorporateActionCode\\\":{\\\"Value\\\":\\\"\\\"},\\\"NotificationSign\\\":{\\\"Value\\\":\\\"\\\"},\\\"OtherSign\\\":{\\\"Value\\\":\\\"\\\"},\\\"AllowNvdr\\\":{\\\"Value\\\":\\\"A\\\"},\\\"AllowShortSell\\\":{\\\"Value\\\":\\\"Y\\\"},\\\"AllowShortSellOnNvdr\\\":{\\\"Value\\\":\\\"Y\\\"},\\\"AllowTtf\\\":{\\\"Value\\\":\\\"N\\\"},\\\"ParValue\\\":{\\\"Value\\\":100000},\\\"FirstTradingDate\\\":{\\\"Value\\\":\\\"0001-01-01T00:00:00\\\"},\\\"FirstTradingTime\\\":{\\\"Hours\\\":0,\\\"Minutes\\\":0,\\\"Seconds\\\":0},\\\"LastTradingDate\\\":{\\\"Value\\\":\\\"0001-01-01T00:00:00\\\"},\\\"LastTradingTime\\\":{\\\"Hours\\\":0,\\\"Minutes\\\":0,\\\"Seconds\\\":0},\\\"MarketSegment\\\":{\\\"Value\\\":\\\"SET\\\"},\\\"PhysicalDelivery\\\":{\\\"Value\\\":\\\"N\\\"},\\\"ContractSize\\\":{\\\"Value\\\":1},\\\"SectorCode\\\":{\\\"Value\\\":\\\"5\\\"},\\\"OriginatesFrom\\\":{\\\"Value\\\":\\\"\\\"},\\\"Status\\\":{\\\"Value\\\":\\\"A\\\"},\\\"Modifier\\\":{\\\"Value\\\":0},\\\"NotationDate\\\":{\\\"Value\\\":\\\"2011-01-01T00:00:00\\\"},\\\"DecimalsInContractSizePQF\\\":{\\\"Value\\\":0},\\\"MsgType\\\":\\\"R\\\",\\\"Metadata\\\":{\\\"Timestamp\\\":1709242340000000000,\\\"Session\\\":\\\"27749f5b23674e3a95293a5c8aa0fa6e\\\",\\\"SequenceNumber\\\":16791}}\",\"MessageType\":\"R\"}";

        var kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = msgR,
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        // Act
        await kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        _itchMapperService.Verify(
            x => x.MapToDataCache(It.IsAny<ItchMessage>(), It.IsAny<Dictionary<string, string>>()),
            Times.AtLeastOnce
        );

        const string msgQ =
            "{\"Message\":\"{\\\"Nanos\\\":{\\\"Value\\\":506888309},\\\"OrderbookId\\\":{\\\"Value\\\":65804},\\\"PriceType\\\":{\\\"Value\\\":11},\\\"Price\\\":{\\\"Value\\\":5750000,\\\"NumberOfDecimals\\\":0},\\\"UpdatedTimestamp\\\":{},\\\"MsgType\\\":\\\"Q\\\",\\\"Metadata\\\":{\\\"Timestamp\\\":1709242340000000000,\\\"Session\\\":\\\"27749f5b23674e3a95293a5c8aa0fa6e\\\",\\\"SequenceNumber\\\":49065}}\",\"MessageType\":\"Q\"}";

        kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = msgQ,
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        // Act
        await kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        _itchMapperService.Verify(
            x => x.MapToDataCache(It.IsAny<ItchMessage>(), It.IsAny<Dictionary<string, string>>()),
            Times.AtLeastOnce
        );

        const string msgk =
            "{\"Message\":\"{\\\"Nanos\\\":{\\\"Value\\\":506917725},\\\"OrderbookId\\\":{\\\"Value\\\":65804},\\\"UpperLimit\\\":{\\\"Value\\\":7475000,\\\"NumberOfDecimals\\\":2},\\\"LowerLimit\\\":{\\\"Value\\\":4025000,\\\"NumberOfDecimals\\\":2},\\\"MsgType\\\":\\\"k\\\",\\\"Metadata\\\":{\\\"Timestamp\\\":1709242340000000000,\\\"Session\\\":\\\"27749f5b23674e3a95293a5c8aa0fa6e\\\",\\\"SequenceNumber\\\":49096}}\",\"MessageType\":\"k\"}";

        kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = msgk,
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        // Act
        await kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        _itchMapperService.Verify(
            x => x.MapToDataCache(It.IsAny<ItchMessage>(), It.IsAny<Dictionary<string, string>>()),
            Times.AtLeastOnce
        );

        const string msgb =
            "{\"Message\":\"{\\\"Nanos\\\":{\\\"Value\\\":744499201},\\\"OrderBookID\\\":{\\\"Value\\\":65804},\\\"MaximumLevel\\\":{\\\"Value\\\":10},\\\"NumberOfLevelItems\\\":{\\\"Value\\\":1},\\\"PriceLevelUpdates\\\":[{\\\"UpdateAction\\\":{\\\"Value\\\":\\\"N\\\"},\\\"Side\\\":{\\\"Value\\\":\\\"A\\\"},\\\"Level\\\":{\\\"Value\\\":1},\\\"Price\\\":{\\\"Value\\\":5700000,\\\"NumberOfDecimals\\\":0},\\\"Quantity\\\":{\\\"Value\\\":121000},\\\"NumberOfDeletes\\\":{\\\"Value\\\":0}}],\\\"MsgType\\\":\\\"b\\\",\\\"Metadata\\\":{\\\"Timestamp\\\":1709242342000000000,\\\"Session\\\":\\\"27749f5b23674e3a95293a5c8aa0fa6e\\\",\\\"SequenceNumber\\\":119907}}\",\"MessageType\":\"b\"}";

        kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = msgb,
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        // Act
        await kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        _itchMapperService.Verify(
            x => x.MapToDataCache(It.IsAny<ItchMessage>(), It.IsAny<Dictionary<string, string>>()),
            Times.AtLeastOnce
        );

        const string msgO =
            "{\"Message\":\"{\\\"Nanos\\\":{\\\"Value\\\":1264738},\\\"OrderBookId\\\":{\\\"Value\\\":65804},\\\"StateName\\\":{\\\"Value\\\":\\\"OPEN1_E\\\"},\\\"MsgType\\\":\\\"O\\\",\\\"Metadata\\\":{\\\"Timestamp\\\":1709261786000000000,\\\"Session\\\":\\\"27749f5b23674e3a95293a5c8aa0fa6e\\\",\\\"SequenceNumber\\\":540894}}\",\"MessageType\":\"O\"}";

        kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = msgO,
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        // Act
        await kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        _itchMapperService.Verify(
            x => x.MapToDataCache(It.IsAny<ItchMessage>(), It.IsAny<Dictionary<string, string>>()),
            Times.AtLeastOnce
        );

        const string msgJ =
            "{\"Message\":\"{\\\"Nanos\\\":{\\\"Value\\\":589686610},\\\"OrderbookId\\\":{\\\"Value\\\":65804},\\\"Value\\\":{\\\"Value\\\":5899,\\\"NumberOfDecimals\\\":0},\\\"HighValue\\\":{\\\"Value\\\":5913,\\\"NumberOfDecimals\\\":0},\\\"LowValue\\\":{\\\"Value\\\":5870,\\\"NumberOfDecimals\\\":0},\\\"OpenValue\\\":{\\\"Value\\\":5894,\\\"NumberOfDecimals\\\":0},\\\"TradedVolume\\\":{\\\"Value\\\":106761000},\\\"TradedValue\\\":{\\\"Value\\\":10079607800,\\\"NumberOfDecimals\\\":0},\\\"Change\\\":{\\\"Value\\\":9,\\\"NumberOfDecimals\\\":0},\\\"ChangePercent\\\":{\\\"Value\\\":15,\\\"NumberOfDecimals\\\":0},\\\"PreviousClose\\\":{\\\"Value\\\":5890,\\\"NumberOfDecimals\\\":0},\\\"Close\\\":{\\\"Value\\\":0,\\\"NumberOfDecimals\\\":0},\\\"Timestamp\\\":{},\\\"MsgType\\\":\\\"J\\\",\\\"Metadata\\\":null}\",\"MessageType\":\"J\"}";

        kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = msgJ,
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        // Act
        await kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        _itchMapperService.Verify(
            x => x.MapToDataCache(It.IsAny<ItchMessage>(), It.IsAny<Dictionary<string, string>>()),
            Times.AtLeastOnce
        );

        const string msgi =
            "{\"Message\":\"{\\\"Nanos\\\":{\\\"Value\\\":589686614},\\\"OrderbookId\\\":{\\\"Value\\\":65804},\\\"DealId\\\":{\\\"Value\\\":783054872583902328},\\\"DealSource\\\":{\\\"Value\\\":2},\\\"Price\\\":{\\\"Value\\\":5750000,\\\"NumberOfDecimals\\\":0},\\\"Quantity\\\":{\\\"Value\\\":1300},\\\"DealDateTime\\\":{},\\\"Action\\\":{\\\"Value\\\":1},\\\"Aggressor\\\":{\\\"Value\\\":\\\"B\\\"},\\\"TradeReportCode\\\":{\\\"Value\\\":0},\\\"DealIdHex\\\":\\\"0ADDF84200009478\\\",\\\"MsgType\\\":\\\"i\\\",\\\"Metadata\\\":{\\\"Timestamp\\\":1709262562000000000,\\\"Session\\\":\\\"27749f5b23674e3a95293a5c8aa0fa6e\\\",\\\"SequenceNumber\\\":906662}}\",\"MessageType\":\"i\"}";

        kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = msgi,
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        // Act
        await kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        _itchMapperService.Verify(
            x => x.MapToDataCache(It.IsAny<ItchMessage>(), It.IsAny<Dictionary<string, string>>()),
            Times.AtLeastOnce
        );

        const string msgI =
            "{\"Message\":\"{\\\"Nanos\\\":{\\\"Value\\\":589686614},\\\"OrderBookId\\\":{\\\"Value\\\":65804},\\\"OpenPrice\\\":{\\\"Value\\\":5675000,\\\"NumberOfDecimals\\\":0},\\\"HighPrice\\\":{\\\"Value\\\":5750000,\\\"NumberOfDecimals\\\":0},\\\"LowPrice\\\":{\\\"Value\\\":5675000,\\\"NumberOfDecimals\\\":0},\\\"LastPrice\\\":{\\\"Value\\\":5750000,\\\"NumberOfDecimals\\\":0},\\\"LastAuctionPrice\\\":{\\\"Value\\\":5675000,\\\"NumberOfDecimals\\\":0},\\\"TurnoverQuantity\\\":{\\\"Value\\\":2219400},\\\"ReportedTurnoverQuantity\\\":{\\\"Value\\\":0},\\\"TurnoverValue\\\":{\\\"Value\\\":12663525000000,\\\"NumberOfDecimals\\\":0},\\\"ReportedTurnoverValue\\\":{\\\"Value\\\":0,\\\"NumberOfDecimals\\\":0},\\\"AveragePrice\\\":{\\\"Value\\\":5705833,\\\"NumberOfDecimals\\\":0},\\\"TotalNumberOfTrades\\\":{\\\"Value\\\":526},\\\"MsgType\\\":\\\"I\\\",\\\"Metadata\\\":{\\\"Timestamp\\\":1709262562000000000,\\\"Session\\\":\\\"27749f5b23674e3a95293a5c8aa0fa6e\\\",\\\"SequenceNumber\\\":906663}}\",\"MessageType\":\"I\"}";

        kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = msgI,
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        // Act
        await kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        _itchMapperService.Verify(
            x => x.MapToDataCache(It.IsAny<ItchMessage>(), It.IsAny<Dictionary<string, string>>()),
            Times.AtLeastOnce
        );
    }

    [Fact]
    public async Task Msg_m_Run_HandleIndexMessageHandler()
    {
        // Arrange
        var msg =
            "{\"Message\":\"{\\\"Nanos\\\":{\\\"Value\\\":111442352},\\\"MarketCode\\\":{\\\"Value\\\":201},\\\"MarketName\\\":{\\\"Value\\\":\\\"Reference Data\\\"},\\\"MarketDescription\\\":{\\\"Value\\\":\\\"REF\\\"},\\\"MsgType\\\":\\\"m\\\",\\\"Metadata\\\":{\\\"Timestamp\\\":1709242340111442352,\\\"Session\\\":\\\"e4f0486695864ad38e3b3f84044e5031\\\",\\\"SequenceNumber\\\":28}}\",\"MessageType\":\"m\"}";

        var kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = msg,
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        // Act
        await kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        _itchMapperService.Verify(
            x => x.MapToDataCache(It.IsAny<ItchMessage>(), It.IsAny<Dictionary<string, string>>()),
            Times.AtLeastOnce
        );
    }

    [Fact]
    public async Task Msg_L_Run_HandleTickSizeTableEntry()
    {
        // Arrange
        var tickSizeTableEntry = new TickSizeTableMessageWrapper
        {
            OrderBookId = new OrderBookId { Value = 123 },
            // Add other necessary properties
        };

        var stockMessage = new StockMessage
        {
            MessageType = "L",
            Message = JsonConvert.SerializeObject(tickSizeTableEntry)
        };

        var message = JsonConvert.SerializeObject(stockMessage);

        _distributedCache.Setup(x => x.GetAsync<string>(It.IsAny<string>()))
            .ReturnsAsync("{}");

        var kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = message,
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        // Act
        await kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        // Assert
        _itchMapperService.Verify(
            x => x.MapToDataCache(It.IsAny<ItchMessage>(), It.IsAny<Dictionary<string, string>>()),
            Times.AtLeastOnce
        );
    }
}