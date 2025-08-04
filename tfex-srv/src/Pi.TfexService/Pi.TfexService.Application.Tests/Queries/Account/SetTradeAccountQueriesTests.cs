using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Common.Features;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Options;
using Pi.TfexService.Application.Providers;
using Pi.TfexService.Application.Queries.Account;
using Pi.TfexService.Application.Queries.Market;
using Pi.TfexService.Application.Services.It;
using Pi.TfexService.Application.Services.MarketData;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Application.Services.UserService;
using Pi.TfexService.Application.Services.Wallet;
using Pi.TfexService.Application.Tests.Mock;
using Pi.TfexService.Application.Utils;
using Pi.TfexService.Domain.Exceptions;
using Pi.TfexService.Domain.Models.InitialMargin;
using PositionItem = Pi.TfexService.Application.Services.It.PositionItem;

namespace Pi.TfexService.Application.Tests.Queries.Account;

public class SetTradeAccountQueriesTest
{
    private readonly Mock<ISetTradeService> _setTradeService;
    private readonly Mock<IMarketDataService> _marketDataService;
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IUserV2Service> _userV2Service;
    private readonly Mock<IItService> _itService;
    private readonly Mock<IFeatureService> _featureService;
    private readonly Mock<IDateTimeProvider> _dateTimeProvider;
    private readonly Mock<IOptionsSnapshot<SymbolOptions>> _mockSymbolOptions = new();
    private readonly SetTradeAccountQueries _setTradeAccountQueries;
    private readonly Mock<IInitialMarginRepository> _initialMarginRepository;
    private readonly Mock<IMarketDataQueries> _marketDataQueries;
    private readonly Mock<ILogger<SetTradeAccountQueries>> _logger;
    private readonly Mock<IWalletService> _walletService;

    public SetTradeAccountQueriesTest()
    {
        _setTradeService = new Mock<ISetTradeService>();
        _marketDataService = new Mock<IMarketDataService>();
        _userService = new Mock<IUserService>();
        _userV2Service = new Mock<IUserV2Service>();
        _itService = new Mock<IItService>();
        _featureService = new Mock<IFeatureService>();
        _dateTimeProvider = new Mock<IDateTimeProvider>();
        _initialMarginRepository = new Mock<IInitialMarginRepository>();
        _marketDataQueries = new Mock<IMarketDataQueries>();
        _walletService = new Mock<IWalletService>();
        _logger = new Mock<ILogger<SetTradeAccountQueries>>();

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
                    EurFutures = (decimal)0.1,
                    JpyFutures = (decimal)0.1,
                    EurUsdFutures = (decimal)0.0001,
                    UsdJpyFutures = (decimal)0.01
                },
                OtherFutures = new OtherFutures()
                {
                    Rss3Futures = (decimal)0.05,
                    Rss3dFutures = (decimal)0.05,
                    JrfFutures = (decimal)0.1,
                }
            },
            LotSize = 1
        };

        _mockSymbolOptions
            .Setup(o => o.Value)
            .Returns(options);

        _setTradeAccountQueries = new SetTradeAccountQueries(
            _setTradeService.Object,
            _marketDataService.Object,
            _userService.Object,
            _userV2Service.Object,
            _itService.Object,
            _featureService.Object,
            _dateTimeProvider.Object,
            _initialMarginRepository.Object,
            _marketDataQueries.Object,
            _walletService.Object,
            _logger.Object
        );
    }

    [Fact]
    public async void GetAccountInto_ShouldThrowExceptionWhenAccountCodeNull()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _setTradeAccountQueries.GetAccountInfo(string.Empty, null!, new CancellationToken()));
    }

    [Fact]
    public async void GetPortfolio_ShouldThrowExceptionWhenAccountCodeNull()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _setTradeAccountQueries.GetPortfolio(null!, "sid", new CancellationToken()));
    }

    [Fact]
    public async void GetAccountInto_ShouldReturnAccountInfoDto()
    {
        _setTradeService.Setup(s => s.GetAccountInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccountInfo(100, 200, 300, 400, 500, 600, 700, "No", 0, 900, 1000, 0, 0, "method"));
        _setTradeService.Setup(s => s.GetPortfolio(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(MockSetTrade.GeneratePortfolioResponse(true));
        _marketDataService.Setup(s => s.GetTicker("sid", It.IsAny<List<string>>())).ReturnsAsync([]);

        var result = await _setTradeAccountQueries.GetAccountInfo(string.Empty, "123456789", new CancellationToken());

        Assert.NotNull(result);
        Assert.Equal(0, result.MarketValue);
        Assert.Equal(0, result.GainsInPortfolio);
        Assert.Equal(0, result.PercentGainsInPortfolio);
        Assert.Equal(200, result.ExcessEquity);
        Assert.Equal(100, result.CreditLine);
        Assert.Equal(400, result.Equity);
        Assert.Equal(500, result.TotalMR);
        Assert.Equal(600, result.TotalMM);
        Assert.Equal(700, result.TotalFM);
        Assert.Equal(0, result.CallForceMargin);
        Assert.Equal(0, result.CallForceMarginMM);
    }

    [Fact]
    public async Task GetAccountInfo_ShouldFallsBackToItService_WhenExceptionIsThrown()
    {
        // Arrange
        var response = new PositionTfexResponseData(
            1, "message", 1, DateTime.Now, new PositionTfex(
                "", "", "", "", "", "", "", 100, 200, 300, 10, 10,
                [new PositionItem("", 1, 0, 10, 1, 0, 35, 0, "THB", 1, 1)]
            )
        );

        _setTradeService.Setup(s => s.GetAccountInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        _setTradeService.Setup(s => s.GetPortfolio(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        _itService.Setup(s => s.GetTfexData(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        _dateTimeProvider.Setup(s => s.GetThDateTimeNow()).Returns(new DateTime(2025, 2, 9));

        // Act
        var result = await _setTradeAccountQueries.GetAccountInfo(string.Empty, "1234567", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAccountInfo_ShouldThrowException_WhenFallsBackToItServiceDuringInvalidDate()
    {
        // Arrange
        var response = new PositionTfexResponseData(
            1, "message", 1, DateTime.Now, new PositionTfex(
                "", "", "", "", "", "", "", 100, 200, 300, 10, 10,
                [new PositionItem("", 1, 0, 10, 1, 0, 35, 0, "THB", 1, 1)]
            )
        );

        _setTradeService.Setup(s => s.GetAccountInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        _setTradeService.Setup(s => s.GetPortfolio(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        _itService.Setup(s => s.GetTfexData(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        _dateTimeProvider.Setup(s => s.GetThDateTimeNow()).Returns(new DateTime(2025, 2, 7));

        await Assert.ThrowsAsync<NotSupportedException>(() => _setTradeAccountQueries.GetAccountInfo(string.Empty, "1234567", CancellationToken.None));
    }

    [Fact]
    public async Task GetAccountInfo_ShouldFallbackWalletExcessEquity_WhenSetTradeAccountNotFound()
    {
        // Arrange
        const string userId = "1234567890";
        const string accountCode = "08002800";
        var setTradeNotFoundException = new SetTradeNotFoundException("Account Not Found");

        _setTradeService.Setup(s => s.GetAccountInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(setTradeNotFoundException);
        _setTradeService.Setup(s => s.GetPortfolio(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(setTradeNotFoundException);
        _walletService.Setup(w => w.GetWalletBalance(userId, accountCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(200m);

        var result = await _setTradeAccountQueries.GetAccountInfo(userId, accountCode, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(0, result.MarketValue);
        Assert.Equal(0, result.GainsInPortfolio);
        Assert.Equal(0, result.PercentGainsInPortfolio);
        Assert.Equal(200m, result.ExcessEquity);
        Assert.Equal(0, result.CreditLine);
        Assert.Equal(0, result.Equity);
        Assert.Equal(0, result.TotalMR);
        Assert.Equal(0, result.TotalMM);
        Assert.Equal(0, result.TotalFM);
        Assert.Equal(0, result.CallForceMargin);
        Assert.Equal(0, result.CallForceMarginMM);
    }

    [Fact]
    public async Task GetPortfolio_ShouldFallsBackToItService_WhenExceptionIsThrown()
    {
        // Arrange
        var response = new PositionTfexResponseData(
            1, "message", 1, DateTime.Now, new PositionTfex(
                "", "", "", "", "", "", "", 100, 200, 300, 10, 10,
                [new PositionItem("", 1, 0, 10, 1, 0, 35, 0, "THB", 1, 1)]
            )
        );

        _setTradeService.Setup(s => s.GetPortfolio(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        _itService.Setup(s => s.GetTfexData(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        _initialMarginRepository
            .Setup(s => s.GetInitialMarginList(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InitialMargin>
            {
                new InitialMargin
                {
                    Symbol = "Underlying",
                    Im = 10,
                    ImSpread = 2,
                    ImOutright = 17,
                    ProductType = "FUT"
                }
            });
        _marketDataService.Setup(s => s.GetTicker(null, It.IsAny<List<string>>())).ReturnsAsync([]);
        _dateTimeProvider.Setup(s => s.GetThDateTimeNow()).Returns(new DateTime(2025, 2, 9));

        // Act
        var result = await _setTradeAccountQueries.GetPortfolio("1234567", null, new CancellationToken());

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetPortfolio_ShouldThrowException_WhenFallsBackToItServiceDuringInvalidDate()
    {
        // Arrange
        var response = new PositionTfexResponseData(
            1, "message", 1, DateTime.Now, new PositionTfex(
                "", "", "", "", "", "", "", 100, 200, 300, 10, 10,
                [new PositionItem("", 1, 0, 10, 1, 0, 35, 0, "THB", 1, 1)]
            )
        );

        _setTradeService.Setup(s => s.GetPortfolio(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        _itService.Setup(s => s.GetTfexData(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        _initialMarginRepository
            .Setup(s => s.GetInitialMarginList(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InitialMargin>
            {
                new InitialMargin
                {
                    Symbol = "Underlying",
                    Im = 10,
                    ImSpread = 2,
                    ImOutright = 17,
                    ProductType = "FUT"
                }
            });
        _marketDataService.Setup(s => s.GetTicker("sid", It.IsAny<List<string>>())).ReturnsAsync([]);
        _dateTimeProvider.Setup(s => s.GetThDateTimeNow()).Returns(new DateTime(2025, 2, 7));

        await Assert.ThrowsAsync<NotSupportedException>(() => _setTradeAccountQueries.GetPortfolio("1234567", null, new CancellationToken()));
    }

    [Fact]
    public async void GetPortfolio_ShouldReturnPortfolioDto()
    {
        _featureService.Setup(s => s.IsOn(Features.UseTfexPlByCost)).Returns(true);
        _initialMarginRepository
            .Setup(s => s.GetInitialMarginList(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InitialMargin>
            {
                new InitialMargin
                {
                    Symbol = "Underlying",
                    Im = 10,
                    ImSpread = 2,
                    ImOutright = 17,
                    ProductType = "FUT"
                }
            });
        _setTradeService.Setup(s => s.GetPortfolio(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(MockSetTrade.GeneratePortfolioResponse(true));
        _marketDataService.Setup(s => s.GetTicker("sid", It.IsAny<List<string>>())).ReturnsAsync([]);

        var result = await _setTradeAccountQueries.GetPortfolio("123456789", "sid", new CancellationToken());

        Assert.NotNull(result);
        Assert.Equal("Symbol", result.First().Symbol);
        Assert.Equal(Side.Long, result.First().Side);
        Assert.Equal(100, result.First().ActualPosition);
        Assert.Equal(10000.0M, result.First().AverageCost);
        Assert.Equal(10000, result.First().MarketValue);
        Assert.Equal(100, result.First().Multiplier);
        Assert.Equal(Currency.THB, result.First().Currency);
        Assert.Equal(0, result.First().GainLoss);
        Assert.Equal(0, result.First().GainLossPercentage);
        Assert.Equal(0, result.First().GainLossRealized);
        Assert.Equal(100 * 17, result.First().MarginRequire);
    }

    [Fact]
    public async Task GetPortfolioByUserId_ShouldReturnPortfolioSummaries_WhenUserHasValidAccounts()
    {
        _setTradeService.Setup(s => s.GetAccountInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccountInfo(100, 200, 300, 400, 500, 600, 700, "No", 0, 900, 1000, 0, 0, "method"));
        _setTradeService.Setup(s => s.GetPortfolio(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(MockSetTrade.GeneratePortfolioResponse(true));
        _marketDataService.Setup(s => s.GetTicker("sid", It.IsAny<List<string>>())).ReturnsAsync([]);

        var externalAccountDetail = new ExternalAccountDetails(Guid.NewGuid(), "", 0);
        var tradingAccountDetail = new TradingAccountDetails(Guid.NewGuid(), "12345670", "", "", "", "", new List<ExternalAccountDetails> { externalAccountDetail });
        var userTradingAccountInfo = new UserTradingAccountInfo("1234567", new List<TradingAccountDetails> { tradingAccountDetail });
        _userService.Setup(s => s.GetTradingAccounts(It.IsAny<string>()))
            .ReturnsAsync([userTradingAccountInfo]);

        // Act
        var result = await _setTradeAccountQueries.GetPortfolioByUserId("123456789", new CancellationToken());

        // Assert
        Assert.Equal("1234567", result.First().CustCode);
        Assert.True(result.First().IsSuccess);
        Assert.Equal(0, result.First().TotalMarketValue);
        Assert.Equal(0, result.First().UpnlPercentage);
        Assert.Equal("12345670", result.First().TradingAccountNo);
        Assert.Equal(0, result.First().Upnl);
        Assert.Equal(300, result.First().CashBalance);
        Assert.Equal(400, result.First().TotalValue);
    }

    [Theory]
    [InlineData("", "", "", true)]
    [InlineData("Symbol", "Logo", "InstrumentCategory", true)]
    [InlineData("", "", "InstrumentCategory", false)]
    [InlineData("Symbol", "Logo", "InstrumentCategory", false)]
    public async void GetPortfolio_ShouldHandleLogoProperly(string mSymbol, string mLogo, string mInstrumentCategory, bool? isLong)
    {
        _initialMarginRepository
            .Setup(s => s.GetInitialMarginList(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InitialMargin>
            {
                new InitialMargin
                {
                    Symbol = "Underlying",
                    Im = 10,
                    ImSpread = 2,
                    ImOutright = 17,
                    ProductType = "FUT"
                }
            });
        _setTradeService.Setup(s => s.GetPortfolio(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(MockSetTrade.GeneratePortfolioResponse(isLong));
        _marketDataService.Setup(s => s.GetTicker("sid", It.IsAny<List<string>>())).ReturnsAsync([MockMarketData.GenerateTicker(mSymbol, mLogo, mInstrumentCategory)]);

        var result = await _setTradeAccountQueries.GetPortfolio("123456789", "sid", new CancellationToken());

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetPortfolioByUserId_ShouldReturnFailedPortfolioSummaries_WhenUserAccountFailed()
    {
        var externalAccountDetail = new ExternalAccountDetails(Guid.NewGuid(), "", 0);
        var tradingAccountDetail = new TradingAccountDetails(Guid.NewGuid(), "12345670", "", "", "", "", new List<ExternalAccountDetails> { externalAccountDetail });
        var userTradingAccountInfo = new UserTradingAccountInfo("1234567", new List<TradingAccountDetails> { tradingAccountDetail });
        _userService.Setup(s => s.GetTradingAccounts(It.IsAny<string>()))
            .ReturnsAsync([userTradingAccountInfo]);
        _setTradeService.Setup(s => s.GetAccountInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _setTradeAccountQueries.GetPortfolioByUserId("123456789", new CancellationToken());

        // Assert
        Assert.Equal("1234567", result.First().CustCode);
        Assert.False(result.First().IsSuccess);
        Assert.Equal(0, result.First().TotalMarketValue);
        Assert.Equal(0, result.First().UpnlPercentage);
        Assert.Equal("12345670", result.First().TradingAccountNo);
        Assert.Equal(0, result.First().Upnl);
        Assert.Equal(0, result.First().CashBalance);
        Assert.Equal(0, result.First().TotalValue);
    }

    [Theory]
    [InlineData("", "", "", null)]
    [InlineData("Symbol", "Logo", "InstrumentCategory", null)]
    public async void GetPortfolio_ShouldThrowErrorWhenHaveSamePosition(string mSymbol, string mLogo, string mInstrumentCategory, bool? isLong)
    {
        _initialMarginRepository
            .Setup(s => s.GetInitialMarginList(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InitialMargin>
            {
                new InitialMargin
                {
                    Symbol = "Underlying",
                    Im = 10,
                    ImSpread = 2,
                    ImOutright = 17,
                    ProductType = "FUT"
                }
            });
        _setTradeService.Setup(s => s.GetPortfolio(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(MockSetTrade.GeneratePortfolioResponse(isLong));
        _marketDataService.Setup(s => s.GetTicker("sid", It.IsAny<List<string>>())).ReturnsAsync([MockMarketData.GenerateTicker(mSymbol, mLogo, mInstrumentCategory)]);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _setTradeAccountQueries.GetPortfolio("123456789", "sid", new CancellationToken()));
    }

    [Fact]
    public async Task GetTradingInfo_Should_Throw_Exception_When_AccountCode_Null()
    {
        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _setTradeAccountQueries.GetSeriesInfo(null!, null!, null!, new CancellationToken()));

        // Assert
        Assert.Equal("Invalid Account Code", exception.Message);
    }

    [Fact]
    public async Task GetTradingInfo_Should_Throw_Exception_When_Symbol_Null()
    {
        // Arrange
        const string accountCode = "1234567890";
        const string sid = "1111111111";

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _setTradeAccountQueries.GetSeriesInfo(accountCode, sid, null!, new CancellationToken()));

        // Assert
        Assert.Equal("Invalid Symbol", exception.Message);
    }

    [Theory]
    [InlineData("SVFA24")]
    [InlineData("SVFB25")]
    [InlineData("S50A24")]
    [InlineData("S50B24")]
    [InlineData("S50A24C700")]
    [InlineData("S50B24C700")]
    public async Task GetTradingInfo_Should_Throw_Exception_When_Invalid_Month_Series(string series)
    {
        // Arrange
        const string accountCode = "1234567890";
        const string sid = "1111111111";

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _setTradeAccountQueries.GetSeriesInfo(accountCode, sid, series, new CancellationToken()));

        // Assert
        Assert.Equal("Invalid Expiration month", exception.Message);
    }

    [Fact]
    public async Task GetTradingInfo_Should_Throw_Exception_When_AccountInfo_Null()
    {
        // Arrange
        const string accountCode = "1234567890";
        const string sid = "1111111111";
        const string symbol = "SVFZ24";

        _setTradeService.Setup(s => s.GetAccountInfo(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => null!);

        // Act
        var exception = await Assert.ThrowsAsync<SetTradeNotFoundException>(async () => await _setTradeAccountQueries.GetSeriesInfo(accountCode, sid, symbol, new CancellationToken()));

        // Assert
        Assert.Equal("Account or Portfolio Not Found", exception.Message);
    }

    [Fact]
    public async Task GetTradingInfo_Should_Throw_Exception_When_Portfolio_Null()
    {
        // Arrange
        const string accountCode = "1234567890";
        const string sid = "1111111111";
        const string symbol = "SVFZ24";

        _setTradeService.Setup(s => s.GetAccountInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccountInfo(100, 200, 300, 400, 500, 600, 700, "No", 0, 900, 1000, 0, 0, "method"));

        _setTradeService.Setup(s => s.GetPortfolio(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => null!);

        // Act
        var exception = await Assert.ThrowsAsync<SetTradeNotFoundException>(async () => await _setTradeAccountQueries.GetSeriesInfo(accountCode, sid, symbol, new CancellationToken()));

        // Assert
        Assert.Equal("Account or Portfolio Not Found", exception.Message);
    }

    [Fact]
    public async Task GetTradingInfo_Should_Throw_Exception_When_MarketData_Null()
    {
        // Arrange
        const string accountCode = "1234567890";
        const string sid = "1111111111";
        const string symbol = "SVFZ24";

        _setTradeService.Setup(s => s.GetAccountInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccountInfo(100, 200, 300, 400, 500, 600, 700, "No", 0, 900, 1000, 0, 0, "method"));
        _setTradeService.Setup(s => s.GetPortfolio(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(MockSetTrade.GeneratePortfolioResponse(true));
        _marketDataService.Setup(s => s.GetTicker("sid", It.IsAny<List<string>>())).ReturnsAsync(() => null!);

        // Act
        var exception = await Assert.ThrowsAsync<NotSupportedException>(async () => await _setTradeAccountQueries.GetSeriesInfo(accountCode, sid, symbol, new CancellationToken()));

        // Assert
        Assert.Equal("Symbol Not Found In Market Data", exception.Message);
    }

    [Theory]
    [InlineData("PreciousMetalFutures", "SVFZ24", true)]
    [InlineData("PreciousMetalFutures", "SVFZ24", false)]
    [InlineData("CurrencyFutures", "USDU24", true)]
    [InlineData("CurrencyFutures", "USDU24", false)]
    [InlineData("", "JPYZ24", true)]
    [InlineData("", "JPYZ24", false)]
    [InlineData(null, "JPYZ24", true)]
    [InlineData(null, "JPYZ24", false)]
    [InlineData("CurrencyFutures", "EURF25", true)]
    [InlineData("CurrencyFutures", "EURF25", false)]
    [InlineData("CurrencyFutures", "JPYZ24", true)]
    [InlineData("CurrencyFutures", "JPYZ24", false)]
    [InlineData("OtherFutures", "RSS3Z24", true)]
    [InlineData("OtherFutures", "RSS3Z24", false)]
    [InlineData("OtherFutures", "RSS3DZ24", true)]
    [InlineData("OtherFutures", "RSS3DZ24", false)]
    [InlineData("OtherFutures", "JRFZ24", true)]
    [InlineData("OtherFutures", "JRFZ24", false)]
    public async Task GetTradingInfo_NotSupportMultiplier_Should_Return_TradingInfo_Without_Multiplier(string instrumentCategory, string symbol, bool isLong)
    {
        // Arrange
        const string accountCode = "1234567890";
        const string sid = "1111111111";

        _setTradeService.Setup(s => s.GetAccountInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccountInfo(100, 200, 300, 400, 500, 600, 700, "No", 0, 900, 1000, 0, 0, "method"));
        _setTradeService.Setup(s => s.GetPortfolio(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(MockSetTrade.GeneratePortfolioResponse(isLong, symbol));
        _marketDataQueries.Setup(s => s.GetMarketData(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(MockMarketData.GenerateMarketDate(symbol, "", instrumentCategory));

        // Act
        var tradingInfo = await _setTradeAccountQueries.GetSeriesInfo(accountCode, sid, symbol, new CancellationToken());

        // Assert
        Assert.NotNull(tradingInfo);
        Assert.Equal(null!, tradingInfo.Multiplier);
        Assert.Equal(null!, tradingInfo.MultiplierType);
        Assert.Equal(null!, tradingInfo.MultiplierUnit);
    }

    private static void OverrideDateUtils(DateTime fakeDate)
    {
        var field = typeof(DateUtils).GetField("ThTimeZoneInfo", BindingFlags.NonPublic | BindingFlags.Static);
        field!.SetValue(null, TimeZoneInfo.CreateCustomTimeZone("FakeTimeZone", new TimeSpan(7, 0, 0), "Fake", "Fake"));

        var method = typeof(DateUtils).GetMethod("GetThDateTimeNow", BindingFlags.Public | BindingFlags.Static);
        method!.Invoke(null, null);
    }
}