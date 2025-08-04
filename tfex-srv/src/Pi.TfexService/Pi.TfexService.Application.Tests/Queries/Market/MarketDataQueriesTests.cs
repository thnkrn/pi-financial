using Microsoft.Extensions.Options;
using Moq;
using Pi.Client.Sirius.Model;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Options;
using Pi.TfexService.Application.Queries.Market;
using Pi.TfexService.Application.Services.MarketData;
using Pi.TfexService.Application.Tests.Mock;

namespace Pi.TfexService.Application.Tests.Queries.Market;

public class MarketDataQueriesTests
{
    private readonly Mock<IMarketDataService> _marketDataService;
    private readonly Mock<IOptionsSnapshot<SymbolOptions>> _mockSymbolOptions = new();
    private readonly MarketDataQueries _marketDataQueries;

    public MarketDataQueriesTests()
    {
        _marketDataService = new Mock<IMarketDataService>();

        var options = new SymbolOptions()
        {
            Multiplier = new Multiplier
            {
                Set50IndexFutures = 200,
                Set50IndexOptions = 200,
                SingleStockFutures = 1000,
                SectorIndexFutures = new SectorIndexFutures()
                {
                    Bank = 1000,
                    Ict = 1000,
                    Energ = 10,
                    Comm = 10,
                    Food = 10
                }
            },
            TickSize = new TickSize()
            {
                Set50IndexFutures = (decimal)0.1,
                Set50IndexOptions = (decimal)0.1,
                SingleStockFutures = (decimal)0.01,
                SectorIndexFutures = new SectorIndexFutures()
                {
                    Bank = (decimal)0.1,
                    Ict = (decimal)0.1,
                    Energ = 1,
                    Comm = 1,
                    Food = 1
                },
                PreciousMetalFutures = new PreciousMetalFutures()
                {
                    GoldFutures = 10,
                    GoldOnlineFutures = (decimal)0.1,
                    SilverOnlineFutures = (decimal)0.01,
                    GoldD = (decimal)0.1
                },
                CurrencyFutures = new CurrencyFutures()
                {
                    UsdFutures = (decimal)0.01,
                    EurUsdFutures = (decimal)0.0001,
                    UsdJpyFutures = (decimal)0.01
                }
            },
            LotSize = 1
        };

        _mockSymbolOptions
            .Setup(o => o.Value)
            .Returns(options);

        _marketDataQueries = new MarketDataQueries(_marketDataService.Object, _mockSymbolOptions.Object);
    }

    [Theory]
    [InlineData("Set50IndexFutures", "S50U24", 0.1, 1, 200, MultiplierType.Multiplier, MultiplierUnit.Point)]
    [InlineData("Set50IndexOptions", "S50U24C700", 0.1, 1, 200, MultiplierType.Multiplier, MultiplierUnit.Point)]
    [InlineData("SingleStockFutures", "AAVU24", 0.01, 1, 1000, MultiplierType.ContractLot, MultiplierUnit.Shares)]
    [InlineData("SectorIndexFutures", "BANKU24", 0.1, 1, 1000, MultiplierType.Multiplier, MultiplierUnit.Point)]
    [InlineData("SectorIndexFutures", "ICTU24", 0.1, 1, 1000, MultiplierType.Multiplier, MultiplierUnit.Point)]
    [InlineData("SectorIndexFutures", "ENERGU24", 1, 1, 10, MultiplierType.Multiplier, MultiplierUnit.Point)]
    [InlineData("SectorIndexFutures", "COMMU24", 1, 1, 10, MultiplierType.Multiplier, MultiplierUnit.Point)]
    [InlineData("SectorIndexFutures", "FOODU24", 1, 1, 10, MultiplierType.Multiplier, MultiplierUnit.Point)]
    public async Task GetMarketData_Should_Return_Data_Both_TickSize_And_Multiplier_Correctly(
        string instrumentCategory,
        string symbol,
        decimal tickSize,
        decimal lotSize,
        decimal multiplier,
        MultiplierType? multiplierType = null,
        MultiplierUnit? multiplierUnit = null)
    {
        // Arrange
        const string accountCode = "1234567890";
        const string sid = "1111111111";

        _marketDataService.Setup(s => s.GetTicker(It.IsAny<string>(), It.IsAny<List<string>>())).ReturnsAsync([MockMarketData.GenerateTicker(symbol, "", instrumentCategory)]);

        // Act
        var marketData = await _marketDataQueries.GetMarketData(sid, symbol, new CancellationToken());

        // Assert
        Assert.NotNull(marketData);
        Assert.Equal(tickSize, marketData.TickSize);
        Assert.Equal(lotSize, marketData.LotSize);
        Assert.Equal(multiplier, marketData.Multiplier);
        Assert.Equal(multiplierType, marketData.MultiplierType);
        Assert.Equal(multiplierUnit, marketData.MultiplierUnit);
    }

    [Theory]
    [InlineData("PreciousMetalFutures", "GFV24", 10, 1)]
    [InlineData("PreciousMetalFutures", "GOZ24", 0.1, 1)]
    [InlineData("PreciousMetalFutures", "SVFZ24", 0.01, 1)]
    [InlineData("PreciousMetalFutures", "GDZ24", 0.1, 1)]
    [InlineData("CurrencyFutures", "USDZ24", 0.01, 1)]
    [InlineData("CurrencyFutures", "EURUSDZ24", 0.0001, 1)]
    [InlineData("CurrencyFutures", "USDJPYZ24", 0.01, 1)]
    public async Task GetMarketData_Should_Return_Data_Both_TickSize_And_Null_Multiplier_Correctly(
        string instrumentCategory,
        string symbol,
        decimal tickSize,
        decimal lotSize)
    {
        // Arrange
        const string accountCode = "1234567890";
        const string sid = "1111111111";

        _marketDataService.Setup(s => s.GetTicker(It.IsAny<string>(), It.IsAny<List<string>>())).ReturnsAsync([MockMarketData.GenerateTicker(symbol, "", instrumentCategory)]);

        // Act
        var marketData = await _marketDataQueries.GetMarketData(sid, symbol, new CancellationToken());

        // Assert
        Assert.NotNull(marketData);
        Assert.Equal(tickSize, marketData.TickSize);
        Assert.Equal(lotSize, marketData.LotSize);
        Assert.Null(marketData.Multiplier);
        Assert.Null(marketData.MultiplierType);
        Assert.Null(marketData.MultiplierUnit);
    }

    [Theory]
    [InlineData("Unstructure", "BB4V24", null, 1)]
    [InlineData("", "BB3Z24", null, 1)]
    [InlineData(null, "BB5Z224", null, 1)]
    public async Task GetMarketData_Should_Return_TickSize_Null_When_Cannot_Specific(string? instrumentCategory, string symbol, decimal? tickSize, decimal lotSize)
    {
        // Arrange
        const string sid = "1111111111";

        _marketDataService.Setup(s => s.GetTicker(It.IsAny<string>(), It.IsAny<List<string>>())).ReturnsAsync([MockMarketData.GenerateTicker(symbol, "", instrumentCategory)]);

        // Act
        var marketData = await _marketDataQueries.GetMarketData(sid, symbol, new CancellationToken());

        // Assert
        Assert.NotNull(marketData);
        Assert.Equal("Others", marketData.InstrumentCategory);
        Assert.Equal(tickSize, marketData.TickSize);
        Assert.Equal(lotSize, marketData.LotSize);
    }

    [Theory]
    [InlineData("Set50IndexFutures", "S50U24", MultiplierType.Multiplier, MultiplierUnit.Point)]
    [InlineData("Set50IndexOptions", "S50U24C700", MultiplierType.Multiplier, MultiplierUnit.Point)]
    [InlineData("SingleStockFutures", "AAVU24", MultiplierType.ContractLot, MultiplierUnit.Shares)]
    [InlineData("SectorIndexFutures", "BANKU24", MultiplierType.Multiplier, MultiplierUnit.Point)]
    [InlineData("SectorIndexFutures", "COMMU24", MultiplierType.Multiplier, MultiplierUnit.Point)]
    [InlineData("SectorIndexFutures", "ENERGU24", MultiplierType.Multiplier, MultiplierUnit.Point)]
    [InlineData("SectorIndexFutures", "FOODU24", MultiplierType.Multiplier, MultiplierUnit.Point)]
    [InlineData("SectorIndexFutures", "ICTU24", MultiplierType.Multiplier, MultiplierUnit.Point)]
    public async Task GetMarketData_SupportMultiplier_Should_Return_TradingInfo_With_Multiplier(
        string instrumentCategory,
        string symbol,
        MultiplierType multiplierType,
        MultiplierUnit multiplierUnit)
    {
        // Arrange
        const string sid = "1111111111";

        _marketDataService.Setup(s => s.GetTicker(It.IsAny<string>(), It.IsAny<List<string>>())).ReturnsAsync([MockMarketData.GenerateTicker(symbol, "", instrumentCategory)]);

        // Act
        var marketData = await _marketDataQueries.GetMarketData(sid, symbol, new CancellationToken());

        // Assert
        Assert.NotNull(marketData);
        Assert.NotNull(marketData.Multiplier);
        Assert.Equal(multiplierType, marketData.MultiplierType);
        Assert.Equal(multiplierUnit, marketData.MultiplierUnit);
    }

    [Fact]
    public async Task GetMarketData_Should_Return_Null_When_Symbol_Not_Found()
    {
        // Arrange
        const string sid = "1111111111";

        _marketDataService.Setup(s => s.GetTicker(It.IsAny<string>(), It.IsAny<List<string>>())).ReturnsAsync(new List<Ticker>());

        // Act
        var marketData = await _marketDataQueries.GetMarketData(sid, "SymbolNotFound", new CancellationToken());

        // Assert
        Assert.Null(marketData);
    }

    [Fact]
    public async Task GetMarketData_Should_Return_Dictionary_When_SymbolList_Not_Empty()
    {
        // Arrange
        const string sid = "1111111111";
        var symbolList = new List<string> { "Symbol1", "Symbol2", "Symbol3" };
        var marketDataList = new List<Ticker>
        {
            MockMarketData.GenerateTicker("Symbol1", "logo 1", "instrumentCategory 1"),
            MockMarketData.GenerateTicker("Symbol2", "logo 2", "instrumentCategory 2"),
            MockMarketData.GenerateTicker("Symbol3", "", "instrumentCategory 3")
        };

        _marketDataService.Setup(s => s.GetTicker(It.IsAny<string>(), It.IsAny<List<string>>())).ReturnsAsync(marketDataList);

        // Act
        var marketData = await _marketDataQueries.GetMarketData(sid, symbolList, new CancellationToken());

        // Assert
        Assert.NotNull(marketData);
        Assert.Equal(3, marketData.Count);
        Assert.Equal("logo 1", marketData["Symbol1"].Logo);
        Assert.Equal("logo 2", marketData["Symbol2"].Logo);
        Assert.Equal("", marketData["Symbol3"].Logo);
    }
}