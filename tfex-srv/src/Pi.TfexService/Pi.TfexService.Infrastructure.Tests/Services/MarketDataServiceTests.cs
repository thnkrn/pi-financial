using Microsoft.Extensions.Logging;
using Moq;
using Pi.Client.Sirius.Api;
using Pi.Client.Sirius.Model;
using Pi.Common.Features;
using Pi.TfexService.Application.Services.MarketData;
using Pi.TfexService.Domain.Exceptions;
using Pi.TfexService.Infrastructure.Services;

namespace Pi.TfexService.Infrastructure.Tests.Services;

public class MarketDataServiceTests
{
    private readonly Mock<ISiriusApi> _siriusApi = new();
    private readonly Mock<IMarketDataApi> _marketDataApi = new();
    private readonly Mock<IFeatureService> _featureService = new();
    private readonly Mock<ILogger<MarketDataService>> _logger = new();
    private readonly MarketDataService _marketDataService;

    public MarketDataServiceTests()
    {
        _marketDataService = new MarketDataService(_marketDataApi.Object, _siriusApi.Object, _featureService.Object, _logger.Object);
    }

    [Theory]
    [InlineData(0, null)]
    [InlineData(10, null)]
    [InlineData(100, null)]
    [InlineData(140, null)]
    [InlineData(200, null)]
    [InlineData(0, "symbol")]
    [InlineData(10, "symbol")]
    [InlineData(100, "symbol")]
    [InlineData(140, "symbol")]
    [InlineData(200, "symbol")]
    public async Task Should_Return_Data_Properly(int numberOfSymbol, string symbol)
    {
        var response = new MarketTickerResponse(null, null, new Tickers([]));
        if (!string.IsNullOrEmpty(symbol))
        {
            response.Response.Data.Add(new Ticker() { Symbol = symbol });
        }

        // Arrange
        _siriusApi.Setup(s => s.CgsV2MarketTickerPostAsync(
                It.IsAny<string>(),
                It.IsAny<MarketTickerRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var marketData = await _marketDataService.GetTicker("sid", GetSymbols(numberOfSymbol));

        // Assert
        Assert.NotNull(marketData);
    }

    [Fact]
    public async Task Should_Return_Data_Properly_When_Ticker_Null()
    {
        var response = new MarketTickerResponse(null, null, null);

        // Arrange
        _siriusApi.Setup(s => s.CgsV2MarketTickerPostAsync(
                It.IsAny<string>(),
                It.IsAny<MarketTickerRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var marketData = await _marketDataService.GetTicker("sid", GetSymbols(100));

        // Assert
        Assert.NotNull(marketData);
    }

    [Fact]
    public async Task Should_Return_Data_Properly_When_Market_Data_Null()
    {
        // Arrange
        _siriusApi.Setup(s => s.CgsV2MarketTickerPostAsync(
                It.IsAny<string>(),
                It.IsAny<MarketTickerRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var marketData = await _marketDataService.GetTicker("sid", GetSymbols(100));

        // Assert
        Assert.NotNull(marketData);
    }

    [Fact]
    public async Task Should_Throw_Exception()
    {
        // Arrange
        _siriusApi.Setup(s => s.CgsV2MarketTickerPostAsync(
                It.IsAny<string>(),
                It.IsAny<MarketTickerRequest>(),
                It.IsAny<CancellationToken>()))
            .Throws(() => new Exception(""));

        // Act & Assert
        await Assert.ThrowsAsync<SiriusApiException>(() => _marketDataService.GetTicker("sid", GetSymbols(100)));
    }

    private static List<string> GetSymbols(int number)
    {
        var symbols = new List<string>();

        for (var i = 1; i <= number; i++)
        {
            symbols.Add($"Symbol_{i}");
        }

        return symbols;
    }
}