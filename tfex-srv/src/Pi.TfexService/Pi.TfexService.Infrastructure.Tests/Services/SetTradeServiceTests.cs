using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Financial.Client.SetTradeOms.Api;
using Pi.Financial.Client.SetTradeOms.Client;
using Pi.Financial.Client.SetTradeOms.Model;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Domain.Exceptions;
using Pi.TfexService.Infrastructure.Models;
using Pi.TfexService.Infrastructure.Options;
using Pi.TfexService.Infrastructure.Services;
using Currency = Pi.Financial.Client.SetTradeOms.Model.Currency;
using Portfolio = Pi.Financial.Client.SetTradeOms.Model.Portfolio;
using PortfolioResponse = Pi.Financial.Client.SetTradeOms.Model.PortfolioResponse;
using Position = Pi.TfexService.Application.Models.Position;
using PriceType = Pi.TfexService.Application.Models.PriceType;
using SecurityType = Pi.Financial.Client.SetTradeOms.Model.SecurityType;
using Side = Pi.TfexService.Application.Models.Side;
using TotalPortfolio = Pi.Financial.Client.SetTradeOms.Model.TotalPortfolio;

namespace Pi.TfexService.Infrastructure.Tests.Services;

public class SetTradeServiceTests
{
    private readonly IDistributedCache _cache;
    private readonly Mock<ISetTradeOmsApi> _setTradeOmsApi;
    private readonly SetTradeService _setTradeService;

    public SetTradeServiceTests()
    {
        Mock<ILogger<SetTradeService>> logger = new();
        Mock<IOptionsSnapshot<SetTradeOptions>> setTradeOptions = new();
        var cacheOptions = new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(cacheOptions);
        _setTradeOmsApi = new Mock<ISetTradeOmsApi>();
        Mock<IBus> bus = new();
        var options = new SetTradeOptions
        {
            ApiKey = "MOCK_API_KEY",
            Application = "MOCK_APPLICATION",
            AppSecret = "TU9DS19BUFBfU0VDUkVU",
            BrokerId = "MOCK_BROKER_ID",
            Host = "MOCK_HOST",
            TimeoutMs = 5000
        };

        setTradeOptions
            .Setup(o => o.Value)
            .Returns(options);

        _setTradeService = new SetTradeService(
            logger.Object,
            setTradeOptions.Object,
            _cache,
            _setTradeOmsApi.Object,
            bus.Object);
    }

    [Fact]
    public async void GetAccessToken_ShouldReturnAccessTokenFromCacheIfExists()
    {
        var mockAccessToken = "MOCK_ACCESS_TOKEN";
        await _cache.SetStringAsync(CacheKeys.SetTradeAccessToken, mockAccessToken);

        var result = await _setTradeService.GetAccessToken();

        Assert.Equal("MOCK_ACCESS_TOKEN", result);
    }

    [Fact]
    public async void GetCurrentAccessToken_ShouldReturnAccessTokenFromApi()
    {
        SetupAuth();
        var result = await _setTradeService.GetAccessToken();

        Assert.Equal("MOCK_ACCESS_TOKEN", result);
    }

    [Fact]
    public async void GetAccountInfo_ShouldReturnAccountInfo()
    {
        SetupAuth();
        var accountInfo = new AccountInfo(100, 200, 300, 400, 500, 600, 700, "No", 0, 900, 1000, 0, 0, "method");

        _setTradeOmsApi.Setup(s =>
                s.GetAccountInfoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccountInfoResponse
            {
                CreditLine = 100,
                ExcessEquity = 200,
                CashBalance = 300,
                Equity = 400,
                TotalMR = 500,
                TotalMM = 600,
                TotalFM = 700,
                CallForceFlag = "No",
                CallForceMargin = 0,
                LiquidationValue = 900,
                DepositWithdrawal = 1000,
                CallForceMarginMM = 0,
                InitialMargin = 0,
                ClosingMethod = "method"
            });

        // Act
        var result = await _setTradeService.GetAccountInfo("123456789", new CancellationToken());

        // Assert
        Assert.Equal(accountInfo.CreditLine, result.CreditLine);
        Assert.Equal(accountInfo.ExcessEquity, result.ExcessEquity);
        Assert.Equal(accountInfo.CashBalance, result.CashBalance);
        Assert.Equal(accountInfo.Equity, result.Equity);
        Assert.Equal(accountInfo.TotalMarginRequire, result.TotalMarginRequire);
        Assert.Equal(accountInfo.TotalMaintenanceMargin, result.TotalMaintenanceMargin);
        Assert.Equal(accountInfo.TotalForceMargin, result.TotalForceMargin);
        Assert.Equal(accountInfo.CallForceFlag, result.CallForceFlag);
        Assert.Equal(accountInfo.CallForceMargin, result.CallForceMargin);
        Assert.Equal(accountInfo.LiquidationValue, result.LiquidationValue);
        Assert.Equal(accountInfo.DepositWithdrawal, result.DepositWithdrawal);
        Assert.Equal(accountInfo.CallForceMarginMM, result.CallForceMarginMM);
        Assert.Equal(accountInfo.InitialMargin, result.InitialMargin);
        Assert.Equal(accountInfo.ClosingMethod, result.ClosingMethod);
    }

    [Fact]
    public async void GetAccountInfo_TokenExpired_ShouldThrowAuthException()
    {
        SetupAuth();
        await _cache.RemoveAsync(CacheKeys.SetTradeAccessToken);

        var apiException = new ApiException(401, "The access token is invalid or has expired", "{\"message\":\"The access token is invalid or has expired\",\"code\":\"API-401\"}");

        _setTradeOmsApi.Setup(s => s.GetAccountInfoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        var exception =
            await Assert.ThrowsAsync<SetTradeAuthException>(() => _setTradeService.GetAccountInfo("123456789", new CancellationToken()));
        Assert.Equal($"Access token is invalid or has expired", exception.Message);
    }

    [Fact]
    public async void GetAccountInfo_AccountNotFound_ShouldThrowSetTradeAccountInfoException()
    {
        SetupAuth();
        const string accountNo = "123456789";

        var apiException = new ApiException(401, "G001: Account not found", "{\"message\":\"G001: Account not found\",\"code\":\"GWD-01\"}");

        _setTradeOmsApi.Setup(s => s.GetAccountInfoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        var exception =
            await Assert.ThrowsAsync<SetTradeNotFoundException>(() => _setTradeService.GetAccountInfo(accountNo, new CancellationToken()));
        Assert.Equal("Account not found", exception.Message);
    }

    [Fact]
    public async void GetAccountInfo_ApiFailed_ShouldThrowSetTradeAccountInfoException()
    {
        SetupAuth();
        const string accountNo = "123456789";

        var apiException = new ApiException(500, "API rate limit exceeded", "{\"message\":\"API rate limit exceeded\",\"code\":\"GWD-02\"}");

        _setTradeOmsApi.Setup(s => s.GetAccountInfoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        var exception =
            await Assert.ThrowsAsync<SetTradeApiException>(() => _setTradeService.GetAccountInfo(accountNo, new CancellationToken()));
        Assert.Equal($"API rate limit exceeded", exception.Message);
    }

    [Fact]
    public async void GetPortfolio_ShouldReturnPortfolioResponse()
    {
        SetupAuth();
        var portfolioResponse = new PortfolioResponse(portfolioList:
        [
            new Portfolio(
                brokerId: "BrokerId",
                accountNo: "AccountNo",
                symbol: "Symbol",
                underlying: "Underlying",
                securityType: SecurityType.FUTURES,
                lastTradingDate: new DateOnly(2023, 12, 31),
                multiplier: 1M,
                currency: Currency.USD,
                currentXRT: 1.0M,
                asOfDateXRT: "2023-12-31",
                hasLongPosition: true,
                startLongPosition: 100,
                actualLongPosition: 100,
                availableLongPosition: 50,
                startLongPrice: 100.0M,
                startLongCost: 10000.0M,
                longAvgPrice: 100.0M,
                longAvgCost: 10000.0M,
                shortAvgCostTHB: 0M,
                longAvgCostTHB: 10000.0M,
                openLongPosition: 100,
                closeLongPosition: 0,
                startXRTLong: 1.0M,
                startXRTLongCost: 10000.0M,
                avgXRTLong: 1.0M,
                avgXRTLongCost: 10000.0M,
                hasShortPosition: false,
                startShortPosition: 0,
                actualShortPosition: 0,
                availableShortPosition: 0,
                startShortPrice: 0,
                startShortCost: 0,
                shortAvgPrice: 0,
                shortAvgCost: 0,
                openShortPosition: 0,
                closeShortPosition: 0,
                startXRTShort: 0,
                startXRTShortCost: 0,
                avgXRTShort: 0,
                avgXRTShortCost: 0,
                marketPrice: 100,
                realizedPL: 0,
                realizedPLByCost: 0,
                realizedPLCurrency: 0,
                realizedPLByCostCurrency: 0,
                shortAmount: 0,
                longAmount: 10000,
                shortAmountByCost: 0,
                longAmountByCost: 10000,
                priceDigit: 2,
                settleDigit: 2,
                longUnrealizePL: 0,
                longUnrealizePLByCost: 0,
                longPercentUnrealizePL: 0,
                longPercentUnrealizePLByCost: 0,
                longOptionsValue: 0,
                longMarketValue: 10000,
                shortUnrealizePL: 0,
                shortPercentUnrealizePL: 0,
                shortUnrealizePLByCost: 0,
                shortPercentUnrealizePLByCost: 0,
                shortOptionsValue: 0,
                shortMarketValue: 0,
                longAvgPriceTHB: 100,
                shortAvgPriceTHB: 0,
                shortAmountCurrency: 0,
                longAmountCurrency: 10000,
                longMarketValueCurrency: 10000,
                shortMarketValueCurrency: 0,
                longUnrealizePLCurrency: 0,
                shortUnrealizePLCurrency: 0,
                longUnrealizedPLByCostCurrency: 0,
                shortUnrealizedPLByCostCurrency: 0,
                longAmountByCostCurrency: 10000,
                shortAmountByCostCurrency: 0
            )
        ], totalPortfolio: new TotalPortfolio(
            amount: 0,
            marketValue: 0,
            amountByCost: 0,
            unrealizePL: 0,
            unrealizePLByCost: 0,
            realizePL: 0,
            realizePLByCost: 0,
            percentUnrealizePL: 0,
            percentUnrealizePLByCost: 0,
            optionsValue: 0));

        _setTradeOmsApi.Setup(s =>
                s.GetPortfolioInvestorAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(portfolioResponse);

        var result = await _setTradeService.GetPortfolio("123456789", new CancellationToken());

        Assert.NotNull(result);
        Assert.Single(result.PortfolioList);

        var resultPortfolio = result.PortfolioList.First();
        Assert.Equal("BrokerId", resultPortfolio.BrokerId);
        Assert.Equal("AccountNo", resultPortfolio.AccountNo);
        Assert.Equal("Symbol", resultPortfolio.Symbol);
        Assert.Equal("Underlying", resultPortfolio.Underlying);
        Assert.Equal(SecurityType.FUTURES.ToString().ToLower(),
            resultPortfolio.SecurityType.ToString().ToLower());
        Assert.Equal(new DateOnly(2023, 12, 31), resultPortfolio.LastTradingDate);
        Assert.Equal(1M, resultPortfolio.Multiplier);
        Assert.Equal(Currency.USD.ToString(), resultPortfolio.Currency.ToString());
        Assert.Equal(1.0M, resultPortfolio.CurrentXrt);
        Assert.Equal("2023-12-31", resultPortfolio.AsOfDateXrt);
        Assert.True(resultPortfolio.HasLongPosition);
        Assert.Equal(100, resultPortfolio.StartLongPosition);
        Assert.Equal(100, resultPortfolio.ActualLongPosition);
        Assert.Equal(50, resultPortfolio.AvailableLongPosition);
        Assert.Equal(100.0M, resultPortfolio.StartLongPrice);
        Assert.Equal(10000.0M, resultPortfolio.StartLongCost);
        Assert.Equal(100.0M, resultPortfolio.LongAvgPrice);
        Assert.Equal(10000.0M, resultPortfolio.LongAvgCost);
        Assert.Equal(0M, resultPortfolio.ShortAvgCostThb);
        Assert.Equal(10000.0M, resultPortfolio.LongAvgCostThb);
        Assert.Equal(100, resultPortfolio.OpenLongPosition);
        Assert.Equal(0, resultPortfolio.CloseLongPosition);
        Assert.Equal(1.0M, resultPortfolio.StartXrtLong);
        Assert.Equal(10000.0M, resultPortfolio.StartXrtLongCost);
        Assert.Equal(1.0M, resultPortfolio.AvgXrtLong);
        Assert.Equal(10000.0M, resultPortfolio.AvgXrtLongCost);
        Assert.False(resultPortfolio.HasShortPosition);
        Assert.Equal(0, resultPortfolio.StartShortPosition);
        Assert.Equal(0, resultPortfolio.ActualShortPosition);
        Assert.Equal(0, resultPortfolio.AvailableShortPosition);
        Assert.Equal(0M, resultPortfolio.StartShortPrice);
        Assert.Equal(0M, resultPortfolio.StartShortCost);
        Assert.Equal(0M, resultPortfolio.ShortAvgPrice);
        Assert.Equal(0M, resultPortfolio.ShortAvgCost);
        Assert.Equal(0, resultPortfolio.OpenShortPosition);
        Assert.Equal(0, resultPortfolio.CloseShortPosition);
        Assert.Equal(0M, resultPortfolio.StartXrtShort);
        Assert.Equal(0M, resultPortfolio.StartXrtShortCost);
        Assert.Equal(0M, resultPortfolio.AvgXrtShort);
        Assert.Equal(0M, resultPortfolio.AvgXrtShortCost);
        Assert.Equal(100.0M, resultPortfolio.MarketPrice);
        Assert.Equal(0M, resultPortfolio.RealizedPl);
        Assert.Equal(0M, resultPortfolio.RealizedPlByCost);
        Assert.Equal(0M, resultPortfolio.RealizedPlCurrency);
        Assert.Equal(0M, resultPortfolio.RealizedPlByCostCurrency);
        Assert.Equal(0M, resultPortfolio.ShortAmount);
        Assert.Equal(10000.0M, resultPortfolio.LongAmount);
        Assert.Equal(0M, resultPortfolio.ShortAmountByCost);
        Assert.Equal(10000.0M, resultPortfolio.LongAmountByCost);
        Assert.Equal(2, resultPortfolio.PriceDigit);
        Assert.Equal(2, resultPortfolio.SettleDigit);
        Assert.Equal(0M, resultPortfolio.LongUnrealizePl);
        Assert.Equal(0M, resultPortfolio.LongUnrealizePlByCost);
        Assert.Equal(0M, resultPortfolio.LongPercentUnrealizePl);
        Assert.Equal(0M, resultPortfolio.LongPercentUnrealizePlByCost);
        Assert.Equal(0M, resultPortfolio.LongOptionsValue);
        Assert.Equal(10000.0M, resultPortfolio.LongMarketValue);
        Assert.Equal(0M, resultPortfolio.ShortUnrealizePl);
        Assert.Equal(0M, resultPortfolio.ShortPercentUnrealizePl);
        Assert.Equal(0M, resultPortfolio.ShortUnrealizePlByCost);
        Assert.Equal(0M, resultPortfolio.ShortPercentUnrealizePlByCost);
        Assert.Equal(0M, resultPortfolio.ShortOptionsValue);
        Assert.Equal(0M, resultPortfolio.ShortMarketValue);
        Assert.Equal(100.0M, resultPortfolio.LongAvgPriceThb);
        Assert.Equal(0M, resultPortfolio.ShortAvgPriceThb);
        Assert.Equal(0M, resultPortfolio.ShortAmountCurrency);
        Assert.Equal(10000.0M, resultPortfolio.LongAmountCurrency);
        Assert.Equal(10000.0M, resultPortfolio.LongMarketValueCurrency);
        Assert.Equal(0M, resultPortfolio.ShortMarketValueCurrency);
        Assert.Equal(0M, resultPortfolio.LongUnrealizePlCurrency);
        Assert.Equal(0M, resultPortfolio.ShortUnrealizePlCurrency);
        Assert.Equal(0M, resultPortfolio.LongUnrealizedPlByCostCurrency);
        Assert.Equal(0M, resultPortfolio.ShortUnrealizedPlByCostCurrency);
        Assert.Equal(10000.0M, resultPortfolio.LongAmountByCostCurrency);
        Assert.Equal(0M, resultPortfolio.ShortAmountByCostCurrency);

        var totalPortfolio = result.TotalPortfolio;
        Assert.Equal(0M, totalPortfolio.Amount);
        Assert.Equal(0M, totalPortfolio.MarketValue);
        Assert.Equal(0M, totalPortfolio.AmountByCost);
        Assert.Equal(0M, totalPortfolio.UnrealizePl);
        Assert.Equal(0M, totalPortfolio.UnrealizePlByCost);
        Assert.Equal(0M, totalPortfolio.RealizePl);
        Assert.Equal(0M, totalPortfolio.RealizePlByCost);
        Assert.Equal(0M, totalPortfolio.PercentUnrealizePl);
        Assert.Equal(0M, totalPortfolio.PercentUnrealizePlByCost);
        Assert.Equal(0M, totalPortfolio.OptionsValue);
    }

    [Fact]
    public async void GetPortfolio_TokenExpired_ShouldThrowAuthException()
    {
        SetupAuth();

        var apiException = new ApiException(401, "error message",
            "{\"message\":\"The access token is invalid or has expired\",\"code\":\"API-401\"}");

        _setTradeOmsApi.Setup(s =>
                s.GetPortfolioInvestorAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        var exception =
            await Assert.ThrowsAsync<SetTradeAuthException>(() => _setTradeService.GetPortfolio("123456789", new CancellationToken()));
        Assert.Equal($"Access token is invalid or has expired", exception.Message);
    }

    [Fact]
    public async void GetPortfolio_AccountNotFound_ShouldThrowSetTradePortfolioException()
    {
        SetupAuth();
        const string accountNo = "123456789";

        var apiException = new ApiException(401, "G001: Account not found", "{\"message\":\"G001: Account not found\",\"code\":\"GWD-01\"}");

        _setTradeOmsApi.Setup(s =>
                s.GetPortfolioInvestorAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        var exception =
            await Assert.ThrowsAsync<SetTradeNotFoundException>(() => _setTradeService.GetPortfolio(accountNo, new CancellationToken()));
        Assert.Equal("Account not found", exception.Message);
    }

    [Fact]
    public async void GetPortfolio_ApiFailed_ShouldThrowSetTradePortfolioException()
    {
        SetupAuth();
        const string accountNo = "123456789";

        var apiException = new ApiException(500, "API rate limit exceeded", "{\"message\":\"API rate limit exceeded\",\"code\":\"GWD-02\"}");

        _setTradeOmsApi.Setup(s =>
                s.GetPortfolioInvestorAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        var exception =
            await Assert.ThrowsAsync<SetTradeApiException>(() => _setTradeService.GetPortfolio(accountNo, new CancellationToken()));
        Assert.Equal($"API rate limit exceeded", exception.Message);
    }

    [Theory]
    [InlineData(1, 1, true)]
    [InlineData(2, 1, false)]
    public async Task GetOrders_ShouldReturnOrders(int page, int pageSize, bool hasNext)
    {
        // Arrange
        SetupAuth();
        const string accountNo = "123456789";

        var orderResponseList = new List<OrderResponse>
        {
            new OrderResponse { OrderNo = 1, AccountNo = accountNo, Symbol = "Symbol1" },
            new OrderResponse { OrderNo = 2, AccountNo = accountNo, Symbol = "Symbol2" }
        };

        var tradeResponseList = new List<TradeResponse>
        {
            new TradeResponse { OrderNo = 1, Px = 10 },
            new TradeResponse { OrderNo = 2, Px = 20 },
        };

        _setTradeOmsApi.Setup(s =>
                s.ListOrderByAccountNoInvestorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderResponseList);
        _setTradeOmsApi.Setup(s => s.ListTradeInvestorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tradeResponseList);

        // Act
        var result = await _setTradeService.GetOrders(accountNo, page, pageSize, null);

        // Assert
        Assert.Single(result.Orders);
        Assert.Equal(page * 10, result.Orders[0].MatchedPrice);
        Assert.Equal(hasNext, result.HasNextPage);
    }

    [Fact]
    public async Task GetActiveOrders_ShouldReturnOrders()
    {
        // Arrange
        SetupAuth();
        const string accountNo = "123456789";

        var orderResponseList = new List<OrderResponse>
        {
            new()  { OrderNo = 1, AccountNo = accountNo, Symbol = "Symbol1", Status = "M" },
            new()  { OrderNo = 2, AccountNo = accountNo, Symbol = "Symbol2", Status = "MP" }
        };

        var tradeResponseList = new List<TradeResponse>
        {
            new () { OrderNo = 1, Px = 10 },
            new () { OrderNo = 2, Px = 20 },
        };

        _setTradeOmsApi.Setup(s =>
                s.ListOrderByAccountNoInvestorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderResponseList);
        _setTradeOmsApi.Setup(s => s.ListTradeInvestorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tradeResponseList);

        // Act
        var result = await _setTradeService.GetActiveOrders(accountNo, "orderNo:desc");

        // Assert
        Assert.Single(result);
        Assert.Equal(2, result[0].OrderNo);
        Assert.Equal("MP", result[0].Status);
    }

    [Theory]
    [InlineData("Access token is invalid or has expired", "{\"message\":\"The access token is invalid or has expired\",\"code\":\"API-401\"}")]
    public async void GetOrders_ShouldThrowAuthException(string expectedMessage, string errorContent)
    {
        // Arrange
        SetupAuth();
        const string accountNo = "123456789";

        var apiException = new ApiException(401, "error message", errorContent);

        _setTradeOmsApi.Setup(s =>
                s.ListOrderByAccountNoInvestorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SetTradeAuthException>(() => _setTradeService.GetOrders(accountNo, 1, 1, null));
        Assert.Equal($"{expectedMessage}", exception.Message);
    }

    [Fact]
    public async void GetOrders_ShouldThrowNotFound()
    {
        // Arrange
        SetupAuth();
        const string accountNo = "123456789";

        var apiException = new ApiException(404, "Order not found", "{\"message\":\"Order not found\",\"code\":\"GWD-02\"}");

        _setTradeOmsApi.Setup(s =>
                s.ListOrderByAccountNoInvestorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SetTradeApiException>(() => _setTradeService.GetOrders(accountNo, 1, 1, null));
        Assert.Equal($"Order not found", exception.Message);
    }

    [Fact]
    public async void GetOrders_ShouldThrowExceptionWhenError()
    {
        // Arrange
        SetupAuth();
        const string accountNo = "123456789";

        _setTradeOmsApi.Setup(s =>
                s.ListOrderByAccountNoInvestorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("API rate limit exceeded"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SetTradeApiException>(() => _setTradeService.GetOrders(accountNo, 1, 1, null));
        Assert.Equal($"SetTradeService GetOrders: Error while calling SetTrade for AccountNo: {accountNo}", exception.Message);
    }

    [Fact]
    public async void GetOrders_ShouldThrowExceptionWhenTradesError()
    {
        // Arrange
        SetupAuth();
        const string accountNo = "123456789";

        var orderResponseList = new List<OrderResponse>
        {
            new OrderResponse { OrderNo = 1, AccountNo = accountNo, Symbol = "Symbol1" },
            new OrderResponse { OrderNo = 2, AccountNo = accountNo, Symbol = "Symbol2" }
        };

        var apiException = new ApiException(404, "Order not found", "{\"message\":\"Trade not found\",\"code\":\"GWD-02\"}");

        _setTradeOmsApi.Setup(s =>
                s.ListOrderByAccountNoInvestorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderResponseList);
        _setTradeOmsApi.Setup(s => s.ListTradeInvestorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SetTradeApiException>(() => _setTradeService.GetOrders(accountNo, 1, 1, null));
        Assert.Equal("Trade not found", exception.Message);
    }

    [Fact]
    public async void GetOrder_ShouldReturnOrder()
    {
        SetupAuth();
        var order = new OrderResponse
        {
            OrderNo = 123,
            TfxOrderNo = "TfxOrderNo",
            AccountNo = "AccountNo",
        };

        var trades = new List<TradeResponse>
        {
            new()
            {
                OrderNo = 123,
                Px = 10
            }
        };

        _setTradeOmsApi.Setup(s =>
                s.GetOrderInvestorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _setTradeOmsApi.Setup(s => s.ListTradeInvestorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(trades);

        var result = await _setTradeService.GetOrderByNo("AccountNo", 123, new CancellationToken());

        Assert.NotNull(result);
        Assert.Equal(123, result.OrderNo);
        Assert.Equal("TfxOrderNo", result.TfxOrderNo);
        Assert.Equal("AccountNo", result.AccountNo);
        Assert.Equal(10, result.MatchedPrice);
    }

    [Fact]
    public async Task GetOrder_ShouldThrowNotFound()
    {
        // Arrange
        SetupAuth();
        const string accountNo = "123456789";
        const long orderNo = 123;

        var apiException = new ApiException(404, "Order not found", "{\"message\":\"Order not found\",\"code\":\"GWD-02\"}");

        _setTradeOmsApi.Setup(s =>
                s.GetOrderInvestorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SetTradeApiException>(() => _setTradeService.GetOrderByNo(accountNo, orderNo, new CancellationToken()));
        Assert.Equal($"Order not found", exception.Message);
    }

    [Theory]
    [InlineData("Access token is invalid or has expired", "{\"message\":\"The access token is invalid or has expired\",\"code\":\"API-401\"}")]
    public async Task GetOrder_ShouldThrowAuthException(string expectedMessage, string errorContent)
    {
        // Arrange
        SetupAuth();
        const string accountNo = "123456789";
        const long orderNo = 123;

        var apiException = new ApiException(401, "error message", errorContent);

        _setTradeOmsApi.Setup(s =>
                s.GetOrderInvestorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SetTradeAuthException>(() => _setTradeService.GetOrderByNo(accountNo, orderNo, new CancellationToken()));
        Assert.Equal($"{expectedMessage}", exception.Message);
    }

    [Fact]
    public async Task GetOrder_ShouldThrowExceptionWhenError()
    {
        // Arrange
        SetupAuth();
        const string accountNo = "123456789";
        const long orderNo = 123;

        _setTradeOmsApi.Setup(s =>
                s.GetOrderInvestorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("API rate limit exceeded"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SetTradeApiException>(() => _setTradeService.GetOrderByNo(accountNo, orderNo, new CancellationToken()));
        Assert.Equal($"SetTradeService GetOrder: Error while calling SetTrade for AccountNo: {accountNo}", exception.Message);
    }

    [Fact]
    public async Task PlaceOrder_ShouldPlaceOrderSuccess()
    {
        // Arrange
        SetupAuth();
        _setTradeOmsApi.Setup(s => s.PlaceOrderInvestorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<PlaceOrderRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OrderResponse(1));

        // Act
        var result = await _setTradeService.PlaceOrder(
            "userId",
            "CustomerCode",
            "AccountCode",
            new SetTradePlaceOrderRequest.PlaceOrderInfo(
                "Symbol",
                Side.Long,
                Position.Auto,
                PriceType.Ato,
                100,
                1,
                0,
                Validity.Day,
                null,
                null,
                null,
                null,
                null,
                true
                ),
            new CancellationToken());

        // Assert
        Assert.Equal(1, result.OrderNo);
    }

    [Theory]
    [InlineData(typeof(SetTradeApiException), "An unknown error occurred", "{\"message\":\"An unknown error occurred\",\"code\":\"GWD-01\"}")]
    [InlineData(typeof(SetTradeApiException), "An unknown error occurred", "{\"message\":null,\"code\":\"GWD-01\"}")]
    [InlineData(typeof(SetTradeApiException), "API rate limit exceeded", "{\"message\":\"API rate limit exceeded\",\"code\":\"GWD-02\"}")]
    [InlineData(typeof(SetTradeApiException), "An unknown error occurred", "{\"message\":null,\"code\":\"GWD-02\"}")]
    [InlineData(typeof(SetTradeAuthException), "Access token is invalid or has expired", "{\"message\":\"The access token is invalid or has expired\",\"code\":\"API-401\"}")]
    [InlineData(typeof(SetTradeInvalidDataException), "Cannot deserialize value of the accepted for Enum class: [MP-MTL, ATO, Limit, MP-MKT]", "{\"message\":\"Message not readable :Cannot deserialize value of the accepted for Enum class: [MP-MTL, ATO, Limit, MP-MKT]\",\"code\": \"GWD-00\"}")]
    [InlineData(typeof(SetTradeInvalidDataException), "Invalid data", "{\"message\": null,\"code\": \"GWD-00\"}")]
    [InlineData(typeof(SetTradeSeriesNotFoundException), "Series not found", "{\"message\":\"G001: Series not found\",\"code\":\"GWD-01\"}")]
    [InlineData(typeof(SetTradeNotFoundException), "Order not found", "{\"message\":\"G001: Order not found\",\"code\": \"GWD-01\"}")]
    [InlineData(typeof(SetTradePriceOutOfRangeException), "Invalid price: should be between 420.0 and 780.0", "{\"message\":\"G003: Invalid price: should be between 420.0 and 780.0\",\"code\":\"GWD-01\"}")]
    [InlineData(typeof(SetTradePriceOutOfRangeFromLastDoneException), "Price is out of range from last done : last done [+/- 5.00%]", "{\"message\":\"O002: Price is out of range from last done : last done [+/- 5.00%]\",\"code\":\"GWD-01\"}")]
    [InlineData(typeof(SetTradePlaceOrderBothSideException), "Cannot open both long and short position in same series", "{\"message\":\"O001: Cannot open both long and short position in same series\",\"code\":\"GWD-01\"}")]
    [InlineData(typeof(SetTradeNotEnoughPositionException), "Not enough position to close : Closable Position[0]", "{\"message\":\"O001: Not enough position to close : Closable Position[0]\",\"code\":\"GWD-01\"}")]
    [InlineData(typeof(SetTradeNotEnoughExcessEquityException), "Not enough excess equity: Excess Equity[-138505.00]", "{\"message\":\"O001: Not enough excess equity: Excess Equity[-138505.00]\",\"code\":\"GWD-01\"}")]
    [InlineData(typeof(SetTradeUpdateOrderNoValueChangedException), "No value changed", "{\"message\":\"O001: No value changed\",\"code\":\"GWD-01\"}")]
    [InlineData(typeof(SetTradeOutsideTradingHoursException), "Not allow to trade during this time", "{\"message\":\"O001: Not allow to trade during this time\",\"code\":\"GWD-01\"}")]
    [InlineData(typeof(SetTradeNotEnoughLineAvailableException), "Exceed line available", "{\"message\":\"O001: Exceed line available\",\"code\":\"GWD-01\"}")]
    public async Task PlaceOrder_ShouldThrowError(Type exceptionType, string message, string errorContent)
    {
        // Arrange
        SetupAuth();

        var apiException = new ApiException(500, message, errorContent);
        _setTradeOmsApi.Setup(s => s.PlaceOrderInvestorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<PlaceOrderRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        // Act & Assert
        var exception = await Assert.ThrowsAnyAsync<Exception>(() => _setTradeService.PlaceOrder(
            "UserId",
            "CustomerCode",
            "AccountCode",
            new SetTradePlaceOrderRequest.PlaceOrderInfo(
                "Symbol",
                Side.Long,
                Position.Auto,
                PriceType.Ato,
                100,
                1,
                0,
                Validity.Day,
                null,
                null,
                null,
                null,
                null,
                true
            ),
            new CancellationToken()));

        Assert.Equal(message, exception.Message);
        Assert.Equal(exceptionType, exception.GetType());
    }

    private async void SetupAuth()
    {
        await _cache.RemoveAsync(CacheKeys.SetTradeAccessToken);

        var authResponse = new SettradeAuthResponse
        {
            AccessToken = "MOCK_ACCESS_TOKEN",
            AuthenticatedUserid = "MOCK_USER_ID",
            BrokerId = "MOCK_BROKER_ID",
            ExpiresIn = 3600,
            RefreshToken = "MOCK_REFRESH_TOKEN",
            TokenType = "Bearer"
        };
        _setTradeOmsApi
            .Setup(
                api => api.AuthLoginAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<SettradeAuthRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(authResponse);
    }

    [Fact]
    public async Task CancelOrder_Should_Returns_True()
    {
        // Arrange
        SetupAuth();
        const string accountNo = "123456789";
        const long orderNo = 123;

        _setTradeOmsApi
            .Setup(s => s.CancelOrderInvestorAsync(It.IsAny<string>(), accountNo, orderNo, It.IsAny<string>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _setTradeService.CancelOrder(accountNo, orderNo);

        // Assert
        Assert.True(response);
    }

    [Fact]
    public async Task CancelOrder_Should_ThrowException_WhenError()
    {
        // Arrange
        SetupAuth();
        const string accountNo = "123456789";
        const long orderNo = 123;

        _setTradeOmsApi
            .Setup(s => s.CancelOrderInvestorAsync(It.IsAny<string>(), accountNo, orderNo, It.IsAny<string>(), default))
            .ThrowsAsync(new ApiException(400, "Error", "{\"message\":\"Invalid Data\",\"code\":\"GWD-00\"}"));

        // Act & Assert
        await Assert.ThrowsAsync<SetTradeInvalidDataException>(() => _setTradeService.CancelOrder(accountNo, orderNo));
    }

    [Fact]
    public async Task PatchOrder_Should_ThrowException_WhenError()
    {
        // Arrange
        SetupAuth();
        const string accountNo = "123456789";
        const long orderNo = 123;
        const decimal price = 100;
        const int volume = 100;

        _setTradeOmsApi
            .Setup(s => s.ChangeOrderInvestorAsync(
                It.IsAny<string>(),
                accountNo,
                orderNo,
                It.IsAny<string>(),
                It.Is<ChangeOrderRequestV3>(r => r.NewPrice == price && r.NewVolume == volume),
                default))
            .ThrowsAsync(new ApiException(400, "Error", "{\"message\":\"Invalid Data\",\"code\":\"GWD-00\"}"));

        // Act & Assert
        await Assert.ThrowsAsync<SetTradeInvalidDataException>(() => _setTradeService.UpdateOrder(accountNo, orderNo, price, volume));
    }

    [Fact]
    public async Task PatchOrder_Should_Returns_True()
    {
        // Arrange
        SetupAuth();
        const string accountNo = "123456789";
        const long orderNo = 123;
        const decimal price = 100;
        const int volume = 100;

        _setTradeOmsApi
            .Setup(s => s.ChangeOrderInvestorAsync(
                It.IsAny<string>(),
                accountNo,
                orderNo,
                It.IsAny<string>(),
                It.Is<ChangeOrderRequestV3>(r => r.NewPrice == price && r.NewVolume == volume),
                default))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _setTradeService.UpdateOrder(accountNo, orderNo, price, volume);

        // Assert
        Assert.True(response);
    }

    [Fact]
    public async void GetTrades_Should_Failed()
    {
        // Arrange
        SetupAuth();
        var accountNo = "08002800";

        _setTradeOmsApi.Setup(s => s.ListTradeInvestorAsync(
            It.IsAny<string>(),
            accountNo,
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(new ApiException(400, "Error", "{\"message\":\"Invalid Data\",\"code\":\"GWD-00\"}"));

        // Act & Assert
        await Assert.ThrowsAsync<SetTradeInvalidDataException>(() => _setTradeService.GetTrades(accountNo, "", new CancellationToken()));
    }

    [Fact]
    public async void GetTrades_Should_Success()
    {
        // Arrange
        SetupAuth();
        var accountNo = "08002800";
        var resp = new List<TradeResponse>
        {
            new()
            {
                OrderNo = 1
            },
            new()
            {
                OrderNo = 2
            }
        };

        _setTradeOmsApi.Setup(s => s.ListTradeInvestorAsync(
            It.IsAny<string>(),
            accountNo,
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(resp);

        // Act
        var response = await _setTradeService.GetTrades(accountNo, "", new CancellationToken());

        // Assert
        Assert.Equal(1, response[0].OrderNo);
        Assert.Equal(2, response[1].OrderNo);
    }
}