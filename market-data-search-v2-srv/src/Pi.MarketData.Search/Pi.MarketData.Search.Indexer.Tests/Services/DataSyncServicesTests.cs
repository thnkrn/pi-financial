using System.Globalization;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenSearch.Client;
using Pi.MarketData.Search.Domain.Entities;
using Pi.MarketData.Search.Domain.Models;
using Pi.MarketData.Search.Indexer.Models;
using Pi.MarketData.Search.Indexer.Services;
using Pi.MarketData.Search.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Search.Indexer.Tests.Services;

public class DataSyncServicesTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IOpenSearchClient> _mockOpenSearchClient;
    private readonly Mock<IOptions<MongoDbSettings>> _mockMongoSettings;
    private readonly Mock<IOptions<OpenSearchSettings>> _mockOpenSearchSettings;
    private readonly Mock<IMongoService<WhiteList>> _mockWhiteListService;
    private readonly Mock<IMongoService<TradingSign>> _mockTradingSignService;
    private readonly Mock<ILogger<DataSyncService>> _mockLogger;
    private readonly DataSyncService _dataSyncService;
    public DataSyncServicesTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockOpenSearchClient = new Mock<IOpenSearchClient>();
        _mockOpenSearchSettings = new Mock<IOptions<OpenSearchSettings>>();
        _mockMongoSettings = new Mock<IOptions<MongoDbSettings>>();
        _mockLogger = new Mock<ILogger<DataSyncService>>();

        _mockWhiteListService = new Mock<IMongoService<WhiteList>>();
        _mockTradingSignService = new Mock<IMongoService<TradingSign>>();

        // Setup service provider to return mongo services
        _mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IMongoService<WhiteList>)))
            .Returns(_mockWhiteListService.Object);
        _mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IMongoService<TradingSign>)))
            .Returns(_mockTradingSignService.Object);

        var mongoSettings = new MongoDbSettings
        {
            SET_TFEX = new MongoDbConfig { Database = "set_tfex_db", Collection = "instruments", ConnectionString = "" },
            GE = new MongoDbConfig { Database = "ge_db", Collection = "instruments", ConnectionString = "" }
        };
        _mockMongoSettings.Setup(m => m.Value).Returns(mongoSettings);

        var openSearchSettings = new OpenSearchSettings { DefaultIndex = "test-index" };
        _mockOpenSearchSettings.Setup(o => o.Value).Returns(openSearchSettings);

        _dataSyncService = new DataSyncService(
            _mockServiceProvider.Object,
            _mockOpenSearchClient.Object,
            _mockMongoSettings.Object,
            _mockOpenSearchSettings.Object,
            _mockLogger.Object
        );
    }

    [Theory]
    [InlineData("X", "disabled")]
    [InlineData("X,T", "disabled")]
    [InlineData("", "enabled")]
    [InlineData("T", "enabled")]
    public async Task CreateSetSearchInstrument_ShouldReturnCorrectResult_WhenTradingSign(string sign, string expected)
    {
        // Arrange
        var orderBookId = 1;
        _mockTradingSignService
            .Setup(service => service.GetOneByFilterAsync(It.IsAny<Expression<Func<TradingSign, bool>>>()))
            .ReturnsAsync(new TradingSign { Sign = sign });
        var document = new SetTfexInstrumentDocument
        {
            OrderBookId = orderBookId
        };

        // Act
        var result = await _dataSyncService.CreateSetSearchInstrument(document);

        // Assert
        Assert.Equal(expected, result.Status);
    }

    [Theory]
    [InlineData("X", true)]
    [InlineData("T,X", true)]
    [InlineData("", false)]
    [InlineData("T", false)]
    public void CheckSetInstrumentStatus_ShouldReturnCorrectResult_WhenTradingSignIsX(string sign, bool expected)
    {
        // Arrange
        var tradingSign = new TradingSign { Sign = sign };
        var instrument = new SetTfexInstrumentDocument();

        // Act
        var result = _dataSyncService.CheckSetInstrumentStatus(tradingSign, instrument);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CheckSetInstrumentStatus_ShouldReturnFalse_WhenLastTradingDateMoreThan5Days()
    {
        // Arrange
        var instrument = new SetTfexInstrumentDocument
        {
            SecurityType = "",
            LastTradingDate = DateTime.UtcNow.AddDays(-7).ToString("dd/MM/yyyy", new CultureInfo("en-US"))
        };

        // Act
        var result = _dataSyncService.CheckSetInstrumentStatus(null, instrument);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CheckSetInstrumentStatus_ShouldReturnFalse_WhenLastTradingDateLessThan5Days()
    {
        // Arrange
        var instrument = new SetTfexInstrumentDocument
        {
            SecurityType = "",
            LastTradingDate = DateTime.UtcNow.ToString("dd/MM/yyyy", new CultureInfo("en-US"))
        };

        // Act
        var result = _dataSyncService.CheckSetInstrumentStatus(null, instrument);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("TSLA", "NASDAQ", "enabled")]
    public async Task CreateGeSearchInstrument_ShouldReturnCorrectResult(string symbol, string exchange, string expected)
    {
        // Arrange
        _mockWhiteListService
            .Setup(service => service.GetAllByFilterAsync(It.IsAny<Expression<Func<WhiteList, bool>>>()))
            .ReturnsAsync([
                new WhiteList
                {
                    Symbol = symbol,
                    Exchange = exchange
                }
            ]);
        var instrument = new GeInstrumentDocument
        {
            Symbol = symbol,
            Exchange = exchange
        };

        // Act
        var result = _dataSyncService.CreateGeSearchInstrument(instrument);

        // Assert
        Assert.Equal(expected, result.Status);
    }
}