using Microsoft.Extensions.Logging;
using Moq;
using Pi.SetService.Application.Exceptions;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Models.AccountSummaries;
using Pi.SetService.Application.Queries;
using Pi.SetService.Application.Services.MarketService;
using Pi.SetService.Application.Services.OnboardService;
using Pi.SetService.Application.Services.OneportService;
using Pi.SetService.Application.Services.PiInternalService;
using Pi.SetService.Application.Services.UserService;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Application.Tests.Queries;

public class SetQueriesTest
{
    private readonly Mock<IInstrumentRepository> _instrumentRepository;
    private readonly Mock<IMarketService> _marketService;
    private readonly Mock<IOnboardService> _onboardService;
    private readonly Mock<IOnePortService> _onePortService;
    private readonly Mock<IPiInternalService> _piInternalService;
    private readonly SetQueries _setQueries;
    private readonly Mock<IUserService> _userService;
    private readonly Mock<ISblOrderRepository> _sblOrderRepository;

    public SetQueriesTest()
    {
        _onboardService = new Mock<IOnboardService>();
        _userService = new Mock<IUserService>();
        _marketService = new Mock<IMarketService>();
        _onePortService = new Mock<IOnePortService>();
        _piInternalService = new Mock<IPiInternalService>();
        _instrumentRepository = new Mock<IInstrumentRepository>();
        _sblOrderRepository = new Mock<ISblOrderRepository>();
        _setQueries = new SetQueries(_onboardService.Object,
            _userService.Object,
            _onePortService.Object,
            _marketService.Object,
            _piInternalService.Object,
            _instrumentRepository.Object,
            _sblOrderRepository.Object,
            Mock.Of<ILogger<SetQueries>>());
    }

    [Fact]
    public async Task Should_ReturnExpected_When_GetCreditBalancePositionsByTradingAccountNoAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.CreditBalance);
        var instruments = new List<EquityInstrument>
        {
            FakeEquityInstrument("EA", 5.10m),
            FakeEquityInstrument("BEYOND", 8.40m),
            FakeEquityInstrument("BANPU", 5.20m),
            FakeEquityInstrument("DELTA", 105.50m)
        };
        var assets = new List<AccountPositionCreditBalance>
        {
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "EA", StockType.Short),
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "EA", StockType.Borrow),
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "EA"),
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "BEYOND"),
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "OR"),
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "NSL-R", StockType.Normal, Ttf.Nvdr),
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "GLD-L"),
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "KF-GOLD"),
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "BANPU") with
            {
                ActualVolume = 0
            },
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "DELTA") with
            {
                ActualVolume = 0,
                TodayRealize = 0
            }
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(q => q.GetPositionsCreditBalance(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(assets);
        _marketService
            .Setup(q => q.GetEquityInstruments(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(instruments);

        // Act
        var actual =
            await _setQueries.GetCreditBalancePositionsByTradingAccountNoAsync(userId,
                tradingAccount.TradingAccountNo);

        // Assert
        Assert.Equal(9, actual.Count);
        Assert.Equal(["BANPU", "BEYOND", "EA", "EA", "EA", "GLD-L", "KF-GOLD", "NSL", "OR"],
            actual.Select(q => q.Symbol));
        Assert.Equal(
            [
                StockType.Normal,
                StockType.Normal,
                StockType.Normal,
                StockType.Short,
                StockType.Borrow,
                StockType.Normal,
                StockType.Normal,
                StockType.Normal,
                StockType.Normal
            ],
            actual.Select(q => q.StockType));
        Assert.Equal([5.20m, 8.40m, 5.10m, 5.10m, 5.10m, 0, 0, 0, 0], actual.Select(q => q.MarketPrice));
        Assert.Contains(null, actual.Select(q => q.InstrumentProfile));
        Assert.Contains(null, actual.Select(q => q.CorporateActions));
    }

    [Fact]
    public async Task Should_ReturnEmpty_When_GetCreditBalancePositionsByTradingAccountNoAsync_NotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.CreditBalance);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(q => q.GetPositionsCreditBalance(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AccountPositionCreditBalance>());

        // Act
        var actual =
            await _setQueries.GetCreditBalancePositionsByTradingAccountNoAsync(userId, tradingAccount.TradingAccountNo);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task
        Should_Error_When_GetCreditBalancePositionsByTradingAccountNoAsync_And_TradingAccountNoInvalidFormat()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccountNo = "08010776";

        // Act
        var act = async () =>
            await _setQueries.GetCreditBalancePositionsByTradingAccountNoAsync(userId, tradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE001, exception.Code);
    }

    [Fact]
    public async Task Should_Error_When_GetCreditBalancePositionsByTradingAccountNoAsync_And_CustCodeNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.CreditBalance);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetCreditBalancePositionsByTradingAccountNoAsync(userId,
                tradingAccount.TradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE101, exception.Code);
    }

    [Fact]
    public async Task
        Should_Error_When_GetCreditBalancePositionsByTradingAccountNoAsync_And_tradingAccountNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.CreditBalance);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetCreditBalancePositionsByTradingAccountNoAsync(userId,
                tradingAccount.TradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE102, exception.Code);
    }

    [Fact]
    public async Task Should_ReturnExpected_When_GetCreditBalanceSummaryByTradingAccountNoAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.CreditBalance);
        var accountBalance = new AvailableCreditBalance
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = tradingAccount.AccountNo,
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Liability = 100004,
            Asset = 100005,
            Equity = 100006,
            MarginRequired = 100007,
            ExcessEquity = 100008,
            Pc = Pc.B,
            CallForce = 40523.75m,
            CallMargin = 47603.85m
        };
        var instruments = new List<EquityInstrument>
        {
            FakeEquityInstrument("EA", 5.10m),
            FakeEquityInstrument("BEYOND", 8.40m),
            FakeEquityInstrument("BANPU", 5.20m),
            FakeEquityInstrument("DELTA", 105.50m)
        };
        var assets = new List<AccountPositionCreditBalance>
        {
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "EA", StockType.Short),
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "EA", StockType.Borrow),
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "EA"),
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "BEYOND"),
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "OR"),
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "NSL-R", StockType.Normal, Ttf.Nvdr),
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "GLD-L"),
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "KF-GOLD"),
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "BANPU") with
            {
                ActualVolume = 0
            },
            FakeAccountPositionCreditBalance(tradingAccount.TradingAccountNo, "DELTA") with
            {
                ActualVolume = 0,
                TodayRealize = 0
            }
        };
        var backofficeAum = new BackofficeAvailableBalance
        {
            TradingAccountNo = "0801078-8",
            AccountNo = "08010788",
            CashBalance = 200000,
            ArTrade = 300,
            ApTrade = 500,
            MarketValue = 10000,
            PostDateTime = default
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(q => q.GetAvailableCreditBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountBalance]);
        _onePortService.Setup(q => q.GetPositionsCreditBalance(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(assets);
        _marketService
            .Setup(q => q.GetEquityInstruments(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(instruments);
        _piInternalService
            .Setup(q => q.GetBackofficeAvailableBalance(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(backofficeAum);

        // Act
        var actual =
            await _setQueries.GetCreditBalanceSummaryByTradingAccountNoAsync(userId,
                tradingAccount.TradingAccountNo);

        // Assert
        Assert.IsType<CreditBalanceAccountSummary>(actual);
        Assert.Equal(accountBalance.Equity, actual.TotalValue);
        Assert.Equal(99907, actual.MarginLoan);
        Assert.Equal(9, actual.Assets.Count());
        Assert.Equal(["BANPU", "BEYOND", "EA", "EA", "EA", "GLD-L", "KF-GOLD", "NSL", "OR"],
            actual.Assets.Select(q => q.Symbol));
        Assert.Equal(
            [
                StockType.Normal,
                StockType.Normal,
                StockType.Normal,
                StockType.Short,
                StockType.Borrow,
                StockType.Normal,
                StockType.Normal,
                StockType.Normal,
                StockType.Normal
            ],
            actual.Assets.Select(q => q.StockType));
        Assert.Equal([5.20m, 8.40m, 5.10m, 5.10m, 5.10m, 0, 0, 0, 0], actual.Assets.Select(q => q.MarketPrice));
        Assert.Contains(null, actual.Assets.Select(q => q.InstrumentProfile));
        Assert.Contains(null, actual.Assets.Select(q => q.CorporateActions));
    }

    [Fact]
    public async Task
        Should_ReturnAssetsEmpty_When_GetCreditBalanceSummaryByTradingAccountNoAsync_And_PositionsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.CreditBalance);
        var accountBalance = new AvailableCreditBalance
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = tradingAccount.AccountNo,
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Liability = 100004,
            Asset = 100005,
            Equity = 100006,
            MarginRequired = 100007,
            ExcessEquity = 100008,
            Pc = Pc.B,
            CallForce = 40523.75m,
            CallMargin = 47603.85m
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(q => q.GetAvailableCreditBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountBalance]);
        _onePortService.Setup(q => q.GetPositionsCreditBalance(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var actual =
            await _setQueries.GetCreditBalanceSummaryByTradingAccountNoAsync(userId,
                tradingAccount.TradingAccountNo);

        // Assert
        Assert.IsType<CreditBalanceAccountSummary>(actual);
        Assert.Equal(accountBalance.Equity, actual.TotalValue);
        Assert.Empty(actual.Assets);
    }

    [Fact]
    public async Task
        Should_Error_When_GetCreditBalanceSummaryByTradingAccountNoAsync_And_AccountBalanceNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.CreditBalance);
        var accountBalance = new AvailableCreditBalance
        {
            TradingAccountNo = "random",
            AccountNo = "random",
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Liability = 100004,
            Asset = 100005,
            Equity = 100006,
            MarginRequired = 100007,
            ExcessEquity = 100008,
            Pc = Pc.B,
            CallForce = 40523.75m,
            CallMargin = 47603.85m
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(q => q.GetAvailableCreditBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountBalance]);

        // Act
        var act = async () =>
            await _setQueries.GetCreditBalanceSummaryByTradingAccountNoAsync(userId,
                tradingAccount.TradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE103, exception.Code);
    }

    [Fact]
    public async Task
        Should_Error_When_GetCreditBalanceSummaryByTradingAccountNoAsync_And_AccountBalanceIsEmpty()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.CreditBalance);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(q => q.GetAvailableCreditBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetCreditBalanceSummaryByTradingAccountNoAsync(userId,
                tradingAccount.TradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE103, exception.Code);
    }

    [Fact]
    public async Task
        Should_Error_When_GetCreditBalanceSummaryByTradingAccountNoAsync_And_TradingAccountNoInvalidFormat()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccountNo = "08010776";

        // Act
        var act = async () =>
            await _setQueries.GetCreditBalanceSummaryByTradingAccountNoAsync(userId, tradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE001, exception.Code);
    }

    [Fact]
    public async Task Should_Error_When_GetCreditBalanceSummaryByTradingAccountNoAsync_And_CustCodeNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.CreditBalance);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetCreditBalanceSummaryByTradingAccountNoAsync(userId,
                tradingAccount.TradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE101, exception.Code);
    }

    [Fact]
    public async Task
        Should_Error_When_GetCreditBalanceSummaryByTradingAccountNoAsync_And_tradingAccountNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount = new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.Cash)
        {
            SblRegistered = false
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetCreditBalanceSummaryByTradingAccountNoAsync(userId,
                tradingAccount.TradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE102, exception.Code);
    }

    private static AccountPositionCreditBalance FakeAccountPositionCreditBalance(string tradingAccountNo, string symbol,
        StockType stockType = StockType.Normal, Ttf ttf = Ttf.None)
    {
        return new AccountPositionCreditBalance(symbol, ttf)
        {
            TradingAccountNo = tradingAccountNo,
            AccountNo = tradingAccountNo.Replace("-", ""),
            StockType = stockType,
            StockTypeChar = StockTypeChar.None,
            StartVolume = 100,
            StartPrice = 5.10m,
            AvailableVolume = 100,
            ActualVolume = 100,
            AvgPrice = 5.10m,
            Amount = 510,
            MR = 100
        };
    }

    private static EquityInstrument FakeEquityInstrument(string symbol, decimal marketPrice)
    {
        return new EquityInstrument
        {
            Symbol = symbol,
            IsNew = false,
            Profile = new InstrumentProfile
            {
                Symbol = symbol,
                Logo = "Mock Logo",
                FriendlyName = "Mock Friendly Name",
                InstrumentCategory = "Thai Equity"
            },
            TradingDetail = new TradingDetail
            {
                MarketPrice = marketPrice,
                Price = marketPrice,
                AvgPrice = 0,
                High = 0,
                Low = 0,
                Open = 0,
                PrevClose = 0,
                Volume = 0,
                Ceiling = 0,
                Floor = 0
            }
        };
    }

    #region GetCreditAccountInfoByTradingAccountNoAsync

    [Fact]
    public async Task Should_ReturnExpected_When_GetCreditAccountInfoByTradingAccountNoAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.CreditBalance);
        var accountBalance = new AvailableCreditBalance
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = tradingAccount.AccountNo,
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Liability = 100004,
            Asset = 100005,
            Equity = 100006,
            MarginRequired = 100007,
            ExcessEquity = 100008,
            Pc = Pc.B,
            CallForce = 40523.75m,
            CallMargin = 47603.85m
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(q => q.GetAvailableCreditBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountBalance]);

        // Act
        var actual =
            await _setQueries.GetCreditAccountInfoByTradingAccountNoAsync(userId, tradingAccount.TradingAccountNo);

        // Assert
        Assert.Equal(accountBalance.CreditLimit, actual.CreditLimit);
        Assert.Equal(accountBalance.BuyCredit, actual.BuyCredit);
        Assert.Equal(accountBalance.CashBalance, actual.CashBalance);
        Assert.Equal(accountBalance.Liability, actual.Liability);
        Assert.Equal(accountBalance.Asset, actual.Asset);
        Assert.Equal(accountBalance.Equity, actual.Equity);
        Assert.Equal(accountBalance.MarginRequired, actual.MarginRequired);
        Assert.Equal(accountBalance.ExcessEquity, actual.ExcessEquity);
        Assert.Equal(accountBalance.CallForce, actual.CallForce);
        Assert.Equal(accountBalance.CallMargin, actual.CallMargin);
    }

    [Fact]
    public async Task Should_Error_When_GetCreditAccountInfoByTradingAccountNoAsync_And_BalancesAreEmpty()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.CreditBalance);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(q => q.GetAvailableCreditBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetCreditAccountInfoByTradingAccountNoAsync(userId, tradingAccount.TradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE103, exception.Code);
    }

    [Fact]
    public async Task
        Should_Error_When_GetCreditAccountInfoByTradingAccountNoAsync_And_TradingAccountNoInvalidFormat()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccountNo = "08010776";

        // Act
        var act = async () =>
            await _setQueries.GetCreditAccountInfoByTradingAccountNoAsync(userId, tradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE001, exception.Code);
    }

    [Fact]
    public async Task Should_Error_When_GetCreditAccountInfoByTradingAccountNoAsync_And_CustCodeNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.CreditBalance);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetCreditAccountInfoByTradingAccountNoAsync(userId, tradingAccount.TradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE101, exception.Code);
    }

    [Fact]
    public async Task Should_Error_When_GetCreditAccountInfoByTradingAccountNoAsync_And_tradingAccountNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount = new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.Cash);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetCreditAccountInfoByTradingAccountNoAsync(userId, tradingAccount.TradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE102, exception.Code);
    }

    #endregion


    #region GetCashAccountInfoByTradingAccountNoAsync

    [Fact]
    public async Task Should_ReturnExpected_When_GetCashAccountInfoByTradingAccountNoAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount = new TradingAccount(Guid.NewGuid(), "0801077", "0801077-1", TradingAccountType.Cash);
        var accountBalance = new AvailableCashBalance
        {
            TradingAccountNo = "random",
            AccountNo = "random",
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Ar = 200,
            Ap = 300,
            ArTrade = 0,
            ApTrade = 0,
            TotalBuyMatch = 0,
            TotalBuyUnmatch = 0
        };
        var backofficeAum = new BackofficeAvailableBalance
        {
            TradingAccountNo = "0801078-8",
            AccountNo = "08010788",
            CashBalance = 200000,
            ArTrade = 300,
            ApTrade = 500,
            MarketValue = 10000,
            PostDateTime = default
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(q => q.GetAvailableCashBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountBalance]);
        _piInternalService
            .Setup(q => q.GetBackofficeAvailableBalance(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(backofficeAum);

        // Act
        var actual =
            await _setQueries.GetCashAccountInfoByTradingAccountNoAsync(userId, tradingAccount.TradingAccountNo);

        // Assert
        Assert.Equal(300, actual.PendingSettlement);
        Assert.Equal(accountBalance.CashBalance, actual.CashBalance);
        Assert.Equal(accountBalance.CreditLimit, actual.CreditLimit);
        Assert.Equal(accountBalance.BuyCredit, actual.BuyCredit);
    }

    [Fact]
    public async Task Should_Error_When_GetCashAccountInfoByTradingAccountNoAsync_And_BalancesAreEmpty()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount = new TradingAccount(Guid.NewGuid(), "0801077", "0801077-1", TradingAccountType.Cash);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(q => q.GetAvailableCashBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetCashAccountInfoByTradingAccountNoAsync(userId, tradingAccount.TradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE103, exception.Code);
    }

    [Fact]
    public async Task
        Should_Error_When_GetCashAccountInfoByTradingAccountNoAsync_And_TradingAccountNoInvalidFormat()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccountNo = "08010776";

        // Act
        var act = async () =>
            await _setQueries.GetCashAccountInfoByTradingAccountNoAsync(userId, tradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE001, exception.Code);
    }

    [Fact]
    public async Task Should_Error_When_GetCashAccountInfoByTradingAccountNoAsync_And_CustCodeNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-1", TradingAccountType.CreditBalance);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetCashAccountInfoByTradingAccountNoAsync(userId, tradingAccount.TradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE101, exception.Code);
    }

    [Fact]
    public async Task Should_Error_When_GetCashAccountInfoByTradingAccountNoAsync_And_tradingAccountNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount = new TradingAccount(Guid.NewGuid(), "0801077", "0801077-1", TradingAccountType.Cash);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetCashAccountInfoByTradingAccountNoAsync(userId, tradingAccount.TradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE102, exception.Code);
    }

    [Fact]
    public async Task
        Should_Error_When_GetCashAccountInfoByTradingAccountNoAsync_And_TradingAccountTypeInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.CreditBalance);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);

        // Act
        var act = async () =>
            await _setQueries.GetCashAccountInfoByTradingAccountNoAsync(userId, tradingAccount.TradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE001, exception.Code);
    }

    [Theory]
    [InlineData("0801077-1", TradingAccountType.Cash)]
    [InlineData("0801077-8", TradingAccountType.CashBalance)]
    public async Task
        Should_Error_When_GetCreditAccountInfoByTradingAccountNoAsync_And_TradingAccountTypeInvalid(
            string tradingAccountNo, TradingAccountType tradingAccountType)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount = new TradingAccount(Guid.NewGuid(), "0801077", tradingAccountNo, tradingAccountType);
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);

        // Act
        var act = async () =>
            await _setQueries.GetCreditAccountInfoByTradingAccountNoAsync(userId, tradingAccount.TradingAccountNo);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE001, exception.Code);
    }

    [Fact]
    public async Task
        Should_ReturnMarginRate_When_GetMarginRateBySymbolFound()
    {
        // Arrange
        var equityMarginInfo = new EquityMarginInfo("BEM", 0.5m, false);

        _instrumentRepository.Setup(q => q.GetEquityMarginInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(equityMarginInfo);

        // Act
        var actual =
            await _setQueries.GetMarginRateBySymbol("BEM");

        // Assert
        Assert.Equal(equityMarginInfo.Rate, actual.Rate);
    }

    [Fact]
    public async Task
        Should_Error_When_GetMarginRateBySymbolNotFound()
    {
        // Arrange
        _instrumentRepository.Setup(q => q.GetEquityMarginInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EquityMarginInfo?)null);

        // Act
        var act = async () =>
            await _setQueries.GetMarginRateBySymbol("BEM");

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE110, exception.Code);
    }

    #endregion

    #region GetAccountInstrumentBalanceAsync

    [Fact]
    public async Task Should_ReturnExpected_When_GetAccountInstrumentBalanceAsync_With_CreditTradingAccountNo()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.CreditBalance);
        var accountBalance = new AvailableCreditBalance
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = "random",
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Liability = 100004,
            Asset = 100005,
            Equity = 100006,
            MarginRequired = 100007,
            ExcessEquity = 100008,
            Pc = Pc.B,
            CallForce = 40523.75m,
            CallMargin = 47603.85m
        };
        var position = new AccountPositionCreditBalance("EA", Ttf.None)
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = tradingAccount.AccountNo,
            StockType = 0,
            StockTypeChar = StockTypeChar.None,
            StartVolume = 0,
            StartPrice = 0,
            AvailableVolume = 100,
            ActualVolume = 0,
            AvgPrice = 0,
            Amount = 0
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(service =>
                service.GetAvailableCreditBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountBalance]);
        _onePortService.Setup(service =>
                service.GetPositionsCreditBalance(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([position]);

        // Act
        var result =
            await _setQueries.GetAccountInstrumentBalanceAsync(userId, tradingAccount.TradingAccountNo,
                position.SecSymbol);

        // Assert
        Assert.Equal(position.SecSymbol, result.Symbol);
        Assert.Equal(accountBalance.ExcessEquity, result.Balance);
        Assert.Equal("THB", result.BalanceUnit);
        Assert.Equal(100, result.Unit);
        Assert.Equal(0, result.NvdrUnit);
        Assert.Equal(0, result.ShortUnit);
        Assert.Equal(0, result.ShortNvdrUnit);
    }

    [Fact]
    public async Task
        Should_ReturnDefaultValue_When_GetAccountInstrumentBalanceAsync_With_CreditTradingAccountNo_And_PositionNotFound()
    {
        // Arrange
        var symbol = "TIDLOR";
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.CreditBalance);
        var accountBalance = new AvailableCreditBalance
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = "random",
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Liability = 100004,
            Asset = 100005,
            Equity = 100006,
            MarginRequired = 100007,
            ExcessEquity = 100008,
            Pc = Pc.B,
            CallForce = 40523.75m,
            CallMargin = 47603.85m
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(service =>
                service.GetAvailableCreditBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountBalance]);
        _onePortService.Setup(service =>
                service.GetPositionsCreditBalance(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result =
            await _setQueries.GetAccountInstrumentBalanceAsync(userId, tradingAccount.TradingAccountNo,
                symbol);

        // Assert
        Assert.Equal(symbol, result.Symbol);
        Assert.Equal(accountBalance.ExcessEquity, result.Balance);
        Assert.Equal("THB", result.BalanceUnit);
        Assert.Equal(0, result.Unit);
        Assert.Equal(0, result.NvdrUnit);
        Assert.Equal(0, result.ShortUnit);
        Assert.Equal(0, result.ShortNvdrUnit);
    }

    [Fact]
    public async Task
        Should_ReturnError_When_GetAccountInstrumentBalanceAsync_With_CreditTradingAccountNo_And_BalanceNotFound()
    {
        // Arrange
        var symbol = "TIDLOR";
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-6", TradingAccountType.CreditBalance);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(service =>
                service.GetAvailableCreditBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _onePortService.Setup(service =>
                service.GetPositionsCreditBalance(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetAccountInstrumentBalanceAsync(userId, tradingAccount.TradingAccountNo,
                symbol);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE103, exception.Code);
    }

    [Fact]
    public async Task
        Should_ReturnExpected_When_GetAccountInstrumentBalanceAsync_With_NonCreditTradingAccountNo()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-8", TradingAccountType.CashBalance);
        var accountBalance = new AvailableCashBalance
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = "random",
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Ar = 0,
            Ap = 0,
            ArTrade = 0,
            ApTrade = 0,
            TotalBuyMatch = 0,
            TotalBuyUnmatch = 0
        };
        var position = new AccountPosition("EA", Ttf.None)
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = tradingAccount.AccountNo,
            StockType = 0,
            StockTypeChar = StockTypeChar.None,
            StartVolume = 0,
            StartPrice = 0,
            AvailableVolume = 100,
            ActualVolume = 0,
            AvgPrice = 0,
            Amount = 0
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(service =>
                service.GetAvailableCashBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountBalance]);
        _onePortService.Setup(service => service.GetPositions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([position]);

        // Act
        var result =
            await _setQueries.GetAccountInstrumentBalanceAsync(userId, tradingAccount.TradingAccountNo,
                position.SecSymbol);

        // Assert
        Assert.Equal(position.SecSymbol, result.Symbol);
        Assert.Equal(accountBalance.BuyCredit, result.Balance);
        Assert.Equal("THB", result.BalanceUnit);
        Assert.Equal(100, result.Unit);
        Assert.Equal(0, result.NvdrUnit);
        Assert.Equal(0, result.ShortUnit);
        Assert.Equal(0, result.ShortNvdrUnit);
    }

    [Fact]
    public async Task
        Should_ReturnDefaultValue_When_GetAccountInstrumentBalanceAsync_With_NonCreditTradingAccountNo_And_PositionNotFound()
    {
        // Arrange
        var symbol = "TIDLOR";
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-8", TradingAccountType.CashBalance);
        var accountBalance = new AvailableCashBalance
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = "random",
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Ar = 0,
            Ap = 0,
            ArTrade = 0,
            ApTrade = 0,
            TotalBuyMatch = 0,
            TotalBuyUnmatch = 0
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(service =>
                service.GetAvailableCashBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountBalance]);
        _onePortService.Setup(service =>
                service.GetPositions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result =
            await _setQueries.GetAccountInstrumentBalanceAsync(userId, tradingAccount.TradingAccountNo,
                symbol);

        // Assert
        Assert.Equal(symbol, result.Symbol);
        Assert.Equal(accountBalance.BuyCredit, result.Balance);
        Assert.Equal("THB", result.BalanceUnit);
        Assert.Equal(0, result.Unit);
        Assert.Equal(0, result.NvdrUnit);
        Assert.Equal(0, result.ShortUnit);
        Assert.Equal(0, result.ShortNvdrUnit);
    }

    [Fact]
    public async Task
        Should_ReturnDefaultValue_When_GetAccountInstrumentBalanceAsync_With_NonCreditTradingAccountNo_And_BalanceNotFound()
    {
        // Arrange
        var symbol = "TIDLOR";
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-8", TradingAccountType.CashBalance);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(service =>
                service.GetAvailableCashBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _onePortService.Setup(service =>
                service.GetPositionsCreditBalance(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetAccountInstrumentBalanceAsync(userId, tradingAccount.TradingAccountNo,
                symbol);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE103, exception.Code);
    }

    [Theory]
    [InlineData("0803177-8")]
    [InlineData("0803177-1")]
    [InlineData("0803177-6")]
    public async Task Should_ReturnError_When_GetAccountInstrumentBalanceAsync_And_TradingAccountFound(
        string tradingAccountNo)
    {
        // Arrange
        var symbol = "TIDLOR";
        var userId = Guid.NewGuid();
        var custCode = "0803177";

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([custCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetAccountInstrumentBalanceAsync(userId, tradingAccountNo, symbol);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE102, exception.Code);
    }

    [Theory]
    [InlineData("0803177-8")]
    [InlineData("0803177-1")]
    [InlineData("0803177-6")]
    public async Task Should_ReturnError_When_GetAccountInstrumentBalanceAsync_And_CustomerNotFound(
        string tradingAccountNo)
    {
        // Arrange
        var symbol = "TIDLOR";
        var userId = Guid.NewGuid();

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetAccountInstrumentBalanceAsync(userId, tradingAccountNo, symbol);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE101, exception.Code);
    }

    [Theory]
    [InlineData("08031778")]
    [InlineData("08031771")]
    [InlineData("08031776")]
    [InlineData("0803177-2")]
    [InlineData("invalid")]
    public async Task Should_ReturnError_When_GetAccountInstrumentBalanceAsync_And_TradingAccountInvalid(
        string tradingAccountNo)
    {
        // Arrange
        var symbol = "TIDLOR";
        var userId = Guid.NewGuid();

        // Act
        var act = async () =>
            await _setQueries.GetAccountInstrumentBalanceAsync(userId, tradingAccountNo, symbol);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE001, exception.Code);
    }

    #endregion

    #region GetAccountSblInstrumentBalanceAsync

    [Theory]
    [InlineData(100000, 5000, 57.50, 100)]
    [InlineData(100000, 8000, 57.50, 100)]
    [InlineData(10000, 900000, 57.50, 10000)]
    [InlineData(10000, 900000, 0, 0)]
    public async Task Should_ReturnExpected_When_GetAccountSblInstrumentBalanceAsync_Success(decimal availableLending,
        decimal ee, decimal closePrice, decimal expected)
    {
        // Arrange
        var symbol = "EA";
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-8", TradingAccountType.CreditBalance)
            {
                SblRegistered = true
            };
        var accountBalance = new AvailableCreditBalance
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = "random",
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Ar = 0,
            Ap = 0,
            Liability = 0,
            Asset = 0,
            Equity = 1000,
            MarginRequired = 0,
            ExcessEquity = ee,
            CallForce = 0,
            CallMargin = 0,
            Pp = 2000
        };
        var position = new AccountPositionCreditBalance(symbol, Ttf.None)
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = tradingAccount.AccountNo,
            StockType = StockType.Short,
            StockTypeChar = StockTypeChar.None,
            StartVolume = 0,
            StartPrice = 0,
            AvailableVolume = 100,
            ActualVolume = 0,
            AvgPrice = 0,
            Amount = 0
        };
        var tradingDetail = new TradingDetail
        {
            Price = 10,
            PrevClose = closePrice
        };
        var sblInstrument = new SblInstrument(Guid.NewGuid(), symbol, 5.00m, 2000000, 1000000, availableLending);
        var marginRate = new EquityMarginInfo(symbol, 70m, false);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(service =>
                service.GetAvailableCreditBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountBalance]);
        _onePortService.Setup(service =>
                service.GetPositionsCreditBalance(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([position]);
        _marketService.Setup(q => q.GetTradingDetail(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tradingDetail);
        _marketService.Setup(q => q.GetCorporateActions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _instrumentRepository.Setup(q => q.GetSblInstrument(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sblInstrument);
        _instrumentRepository.Setup(q => q.GetEquityMarginInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(marginRate);

        // Act
        var result =
            await _setQueries.GetAccountSblInstrumentBalanceAsync(userId, tradingAccount.TradingAccountNo, symbol);

        // Assert
        Assert.Equal(symbol, result.Symbol);
        Assert.True(result.SblEnabled);
        Assert.Equal(accountBalance.ExcessEquity, result.ExcessEquity);
        Assert.Equal(accountBalance.Pp, result.PurchesingPower);
        Assert.Equal(sblInstrument.AvailableLending, result.AvailableLending);
        Assert.Equal(tradingDetail.PrevClose, result.ClosePrice);
        Assert.True(result.AllowBorrowing);
        Assert.Equal(100, result.ShortUnit);
        Assert.Equal(marginRate.Rate, result.MarginRate);
        Assert.Equal(expected, result.MaximumShares);
    }

    [Fact]
    public async Task Should_ReturnExpected_When_GetAccountSblInstrumentBalanceAsync_And_MarginInfoNotFound()
    {
        // Arrange
        var symbol = "EA";
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-8", TradingAccountType.CreditBalance)
            {
                SblRegistered = true
            };
        var accountBalance = new AvailableCreditBalance
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = "random",
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Ar = 0,
            Ap = 0,
            Liability = 0,
            Asset = 0,
            Equity = 1000,
            MarginRequired = 0,
            ExcessEquity = 900000,
            CallForce = 0,
            CallMargin = 0,
            Pp = 2000
        };
        var position = new AccountPositionCreditBalance(symbol, Ttf.None)
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = tradingAccount.AccountNo,
            StockType = StockType.Short,
            StockTypeChar = StockTypeChar.None,
            StartVolume = 0,
            StartPrice = 0,
            AvailableVolume = 100,
            ActualVolume = 0,
            AvgPrice = 0,
            Amount = 0
        };
        var tradingDetail = new TradingDetail
        {
            Price = 10,
            PrevClose = 9.5m
        };
        var sblInstrument = new SblInstrument(Guid.NewGuid(), symbol, 5.00m, 2000000, 1000000, 10000000);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(service =>
                service.GetAvailableCreditBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountBalance]);
        _onePortService.Setup(service =>
                service.GetPositionsCreditBalance(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([position]);
        _marketService.Setup(q => q.GetTradingDetail(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tradingDetail);
        _marketService.Setup(q => q.GetCorporateActions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _instrumentRepository.Setup(q => q.GetSblInstrument(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sblInstrument);
        _instrumentRepository.Setup(q => q.GetEquityMarginInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EquityMarginInfo?)null);

        // Act
        var result =
            await _setQueries.GetAccountSblInstrumentBalanceAsync(userId, tradingAccount.TradingAccountNo, symbol);

        // Assert
        Assert.Equal(symbol, result.Symbol);
        Assert.False(result.SblEnabled);
        Assert.Equal(accountBalance.ExcessEquity, result.ExcessEquity);
        Assert.Equal(accountBalance.Pp, result.PurchesingPower);
        Assert.Equal(0, result.AvailableLending);
        Assert.Equal(tradingDetail.PrevClose, result.ClosePrice);
        Assert.False(result.AllowBorrowing);
        Assert.Equal(100, result.ShortUnit);
        Assert.Equal(0, result.MarginRate);
        Assert.Equal(0, result.MaximumShares);
    }

    [Fact]
    public async Task Should_ReturnExpected_When_GetAccountSblInstrumentBalanceAsync_And_SblInstrumentNotFound()
    {
        // Arrange
        var symbol = "EA";
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-8", TradingAccountType.CreditBalance)
            {
                SblRegistered = true
            };
        var accountBalance = new AvailableCreditBalance
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = "random",
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Ar = 0,
            Ap = 0,
            Liability = 0,
            Asset = 0,
            Equity = 1000,
            MarginRequired = 0,
            ExcessEquity = 900000,
            CallForce = 0,
            CallMargin = 0,
            Pp = 2000
        };
        var position = new AccountPositionCreditBalance(symbol, Ttf.None)
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = tradingAccount.AccountNo,
            StockType = StockType.Short,
            StockTypeChar = StockTypeChar.None,
            StartVolume = 0,
            StartPrice = 0,
            AvailableVolume = 100,
            ActualVolume = 0,
            AvgPrice = 0,
            Amount = 0
        };
        var tradingDetail = new TradingDetail
        {
            Price = 10,
            PrevClose = 9.5m
        };
        var marginRate = new EquityMarginInfo(symbol, 70m, false);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(service =>
                service.GetAvailableCreditBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountBalance]);
        _onePortService.Setup(service =>
                service.GetPositionsCreditBalance(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([position]);
        _marketService.Setup(q => q.GetTradingDetail(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tradingDetail);
        _marketService.Setup(q => q.GetCorporateActions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _instrumentRepository.Setup(q => q.GetSblInstrument(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SblInstrument?)null);
        _instrumentRepository.Setup(q => q.GetEquityMarginInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(marginRate);

        // Act
        var result =
            await _setQueries.GetAccountSblInstrumentBalanceAsync(userId, tradingAccount.TradingAccountNo, symbol);

        // Assert
        Assert.Equal(symbol, result.Symbol);
        Assert.False(result.SblEnabled);
        Assert.Equal(accountBalance.ExcessEquity, result.ExcessEquity);
        Assert.Equal(accountBalance.Pp, result.PurchesingPower);
        Assert.Equal(0, result.AvailableLending);
        Assert.Equal(tradingDetail.PrevClose, result.ClosePrice);
        Assert.False(result.AllowBorrowing);
        Assert.Equal(100, result.ShortUnit);
        Assert.Equal(0, result.MarginRate);
        Assert.Equal(0, result.MaximumShares);
    }

    [Fact]
    public async Task Should_ReturnExpected_When_GetAccountSblInstrumentBalanceAsync_And_CAIsNotEmpty()
    {
        // Arrange
        var symbol = "EA";
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-8", TradingAccountType.CreditBalance)
            {
                SblRegistered = true
            };
        var accountBalance = new AvailableCreditBalance
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = "random",
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Ar = 0,
            Ap = 0,
            Liability = 0,
            Asset = 0,
            Equity = 1000,
            MarginRequired = 0,
            ExcessEquity = 1000000,
            CallForce = 0,
            CallMargin = 0,
            Pp = 2000
        };
        var position = new AccountPositionCreditBalance(symbol, Ttf.None)
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = tradingAccount.AccountNo,
            StockType = StockType.Short,
            StockTypeChar = StockTypeChar.None,
            StartVolume = 0,
            StartPrice = 0,
            AvailableVolume = 100,
            ActualVolume = 0,
            AvgPrice = 0,
            Amount = 0
        };
        var tradingDetail = new TradingDetail
        {
            Price = 10,
            PrevClose = 9.5m
        };
        var corporateAction = new CorporateAction
        {
            Date = default,
            CaType = "XD"
        };
        var sblInstrument = new SblInstrument(Guid.NewGuid(), symbol, 5.00m, 2000000, 1000000, 1000);
        var marginRate = new EquityMarginInfo(symbol, 70m, false);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(service =>
                service.GetAvailableCreditBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountBalance]);
        _onePortService.Setup(service =>
                service.GetPositionsCreditBalance(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([position]);
        _marketService.Setup(q => q.GetTradingDetail(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tradingDetail);
        _marketService.Setup(q => q.GetCorporateActions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([corporateAction]);
        _instrumentRepository.Setup(q => q.GetSblInstrument(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sblInstrument);
        _instrumentRepository.Setup(q => q.GetEquityMarginInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(marginRate);

        // Act
        var result =
            await _setQueries.GetAccountSblInstrumentBalanceAsync(userId, tradingAccount.TradingAccountNo, symbol);

        // Assert
        Assert.Equal(symbol, result.Symbol);
        Assert.True(result.SblEnabled);
        Assert.Equal(accountBalance.ExcessEquity, result.ExcessEquity);
        Assert.Equal(accountBalance.Pp, result.PurchesingPower);
        Assert.Equal(sblInstrument.AvailableLending, result.AvailableLending);
        Assert.Equal(tradingDetail.PrevClose, result.ClosePrice);
        Assert.False(result.AllowBorrowing);
        Assert.Equal(100, result.ShortUnit);
        Assert.Equal(marginRate.Rate, result.MarginRate);
        Assert.Equal(sblInstrument.AvailableLending, result.MaximumShares);
    }

    [Fact]
    public async Task Should_ReturnExpected_When_GetAccountSblInstrumentBalanceAsync_And_PositionIsEmpty()
    {
        // Arrange
        var symbol = "EA";
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-8", TradingAccountType.CreditBalance)
            {
                SblRegistered = true
            };
        var accountBalance = new AvailableCreditBalance
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = "random",
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Ar = 0,
            Ap = 0,
            Liability = 0,
            Asset = 0,
            Equity = 1000,
            MarginRequired = 0,
            ExcessEquity = 1000000,
            CallForce = 0,
            CallMargin = 0,
            Pp = 2000
        };
        var tradingDetail = new TradingDetail
        {
            Price = 10,
            PrevClose = 9.5m
        };
        var sblInstrument = new SblInstrument(Guid.NewGuid(), symbol, 5.00m, 2000000, 1000000, 1000);
        var marginRate = new EquityMarginInfo(symbol, 70m, false);

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(service =>
                service.GetAvailableCreditBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountBalance]);
        _onePortService.Setup(service =>
                service.GetPositionsCreditBalance(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _marketService.Setup(q => q.GetTradingDetail(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tradingDetail);
        _marketService.Setup(q => q.GetCorporateActions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _instrumentRepository.Setup(q => q.GetSblInstrument(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sblInstrument);
        _instrumentRepository.Setup(q => q.GetEquityMarginInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(marginRate);

        // Act
        var result =
            await _setQueries.GetAccountSblInstrumentBalanceAsync(userId, tradingAccount.TradingAccountNo, symbol);

        // Assert
        Assert.Equal(symbol, result.Symbol);
        Assert.True(result.SblEnabled);
        Assert.Equal(accountBalance.ExcessEquity, result.ExcessEquity);
        Assert.Equal(accountBalance.Pp, result.PurchesingPower);
        Assert.Equal(sblInstrument.AvailableLending, result.AvailableLending);
        Assert.Equal(tradingDetail.PrevClose, result.ClosePrice);
        Assert.True(result.AllowBorrowing);
        Assert.Equal(0, result.ShortUnit);
        Assert.Equal(marginRate.Rate, result.MarginRate);
        Assert.Equal(sblInstrument.AvailableLending, result.MaximumShares);
    }

    [Fact]
    public async Task Should_ReturnError_When_GetAccountSblInstrumentBalanceAsync_And_TradingDetailNotFound()
    {
        // Arrange
        var symbol = "TIDLOR";
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-8", TradingAccountType.CreditBalance)
            {
                SblRegistered = true
            };
        var accountBalance = new AvailableCreditBalance
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = "random",
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Ar = 0,
            Ap = 0,
            Liability = 0,
            Asset = 0,
            Equity = 1000,
            MarginRequired = 0,
            ExcessEquity = 1000000,
            CallForce = 0,
            CallMargin = 0,
            Pp = 2000
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(service =>
                service.GetAvailableCreditBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([accountBalance]);
        _marketService.Setup(q => q.GetTradingDetail(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TradingDetail?)null);

        // Act
        var act = async () =>
            await _setQueries.GetAccountSblInstrumentBalanceAsync(userId, tradingAccount.TradingAccountNo, symbol);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE104, exception.Code);
    }

    [Fact]
    public async Task Should_ReturnError_When_GetAccountSblInstrumentBalanceAsync_And_AccountBalanceNotFound()
    {
        // Arrange
        var symbol = "TIDLOR";
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-8", TradingAccountType.CreditBalance)
            {
                SblRegistered = true
            };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);
        _onePortService.Setup(service =>
                service.GetAvailableCreditBalances(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetAccountSblInstrumentBalanceAsync(userId, tradingAccount.TradingAccountNo, symbol);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE103, exception.Code);
    }

    [Fact]
    public async Task Should_ReturnError_When_GetAccountSblInstrumentBalanceAsync_And_SblDisabled()
    {
        // Arrange
        var symbol = "TIDLOR";
        var userId = Guid.NewGuid();
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0801077", "0801077-8", TradingAccountType.CreditBalance)
            {
                SblRegistered = false
            };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([tradingAccount.CustomerCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([tradingAccount]);

        // Act
        var act = async () =>
            await _setQueries.GetAccountSblInstrumentBalanceAsync(userId, tradingAccount.TradingAccountNo, symbol);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE115, exception.Code);
    }

    [Theory]
    [InlineData("0803177-8")]
    [InlineData("0803177-1")]
    [InlineData("0803177-6")]
    public async Task Should_ReturnError_When_GetAccountSblInstrumentBalanceAsync_And_TradingAccountFound(
        string tradingAccountNo)
    {
        // Arrange
        var symbol = "TIDLOR";
        var userId = Guid.NewGuid();
        var custCode = "0803177";

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([custCode]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetAccountSblInstrumentBalanceAsync(userId, tradingAccountNo, symbol);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE102, exception.Code);
    }

    [Theory]
    [InlineData("0803177-8")]
    [InlineData("0803177-1")]
    [InlineData("0803177-6")]
    public async Task Should_ReturnError_When_GetAccountSblInstrumentBalanceAsync_And_CustomerNotFound(
        string tradingAccountNo)
    {
        // Arrange
        var symbol = "TIDLOR";
        var userId = Guid.NewGuid();

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([]);

        // Act
        var act = async () =>
            await _setQueries.GetAccountSblInstrumentBalanceAsync(userId, tradingAccountNo, symbol);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE101, exception.Code);
    }

    [Theory]
    [InlineData("08031778")]
    [InlineData("08031771")]
    [InlineData("08031776")]
    [InlineData("0803177-2")]
    [InlineData("invalid")]
    public async Task Should_ReturnError_When_GetAccountSblInstrumentBalanceAsync_And_TradingAccountInvalid(
        string tradingAccountNo)
    {
        // Arrange
        var symbol = "TIDLOR";
        var userId = Guid.NewGuid();

        // Act
        var act = async () =>
            await _setQueries.GetAccountSblInstrumentBalanceAsync(userId, tradingAccountNo, symbol);

        // Assert
        var exception = await Assert.ThrowsAsync<SetException>(act);
        Assert.Equal(SetErrorCode.SE001, exception.Code);
    }

    #endregion
}
