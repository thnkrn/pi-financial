using Microsoft.Extensions.Logging;
using Moq;
using Pi.Common.ExtensionMethods;
using Pi.Common.Features;
using Pi.PortfolioService.API.Services;
using Pi.PortfolioService.DomainModels;
using Pi.PortfolioService.Models;
using Pi.PortfolioService.Services;

namespace Pi.PortfolioService.API.Tests;

public class PortfolioManagerTest
{
    private readonly Mock<ISiriusService> _siriusService;
    private readonly Mock<IGeService> _geService;
    private readonly Mock<IStructureNoteService> _structureNoteService;
    private readonly Mock<IFeatureService> _featureService;
    private readonly Mock<IFundService> _fundService;
    private readonly Mock<ISetService> _setService;
    private readonly Mock<ITfexService> _tfexService;
    private readonly Mock<IBondService> _bondService;
    private readonly Mock<IUserService> _userService;
    private readonly PortfolioManager _portfolioManager;

    public PortfolioManagerTest()
    {
        _siriusService = new Mock<ISiriusService>();
        _geService = new Mock<IGeService>();
        _structureNoteService = new Mock<IStructureNoteService>();
        _featureService = new Mock<IFeatureService>();
        _fundService = new Mock<IFundService>();
        _setService = new Mock<ISetService>();
        _tfexService = new Mock<ITfexService>();
        _bondService = new Mock<IBondService>();
        _userService = new Mock<IUserService>();
        _portfolioManager = new PortfolioManager(_siriusService.Object,
            _geService.Object,
            _structureNoteService.Object,
            _featureService.Object,
            _fundService.Object,
            _setService.Object,
            _tfexService.Object,
            _bondService.Object,
            _userService.Object,
            Mock.Of<ILogger<PortfolioManager>>());
    }

    [Fact]
    public async Task Should_FetchAsExpected_When_FetchTradingAccountsIsEnabled_And_TradingAccountsHadAllProducts()
    {
        // Arrange
        _featureService.Setup(q => q.IsOff(It.IsAny<string>()))
            .Returns(false);
        _featureService.Setup(q => q.IsOn(It.IsAny<string>()))
            .Returns(true);
        _userService.Setup(q => q.GetTradingAccountsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                FakeTradingAccount(Product.Unknown),
                FakeTradingAccount(Product.Cash),
                FakeTradingAccount(Product.CashBalance),
                FakeTradingAccount(Product.CreditBalanceSbl),
                FakeTradingAccount(Product.Crypto),
                FakeTradingAccount(Product.Derivatives),
                FakeTradingAccount(Product.GlobalEquities),
                FakeTradingAccount(Product.Funds),
                FakeTradingAccount(Product.Bond),
                FakeTradingAccount(Product.CashSbl),
                FakeTradingAccount(Product.CashBalanceSbl),
                FakeTradingAccount(Product.CreditBalance),
                FakeTradingAccount(Product.StructureNoteOnShore),
                FakeTradingAccount(Product.Dr),
                FakeTradingAccount(Product.LiveX),
                FakeTradingAccount(Product.BorrowCash),
                FakeTradingAccount(Product.BorrowCashBalance)
            ]);
        _bondService.Setup(q => q.GetAccountsOverview(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _setService.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _fundService.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _geService.Setup(q => q.GetAccounts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _tfexService.Setup(q => q.GetPortfolioAccount(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _structureNoteService.Setup(q =>
                q.GetStructureNotesPortfolioAccount(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var actual =
            await _portfolioManager.GetPortfolioSummaryV2Async(Guid.NewGuid().ToString(), "someValue",
                CancellationToken.None);

        // Assert
        Assert.IsType<PortfolioSummary>(actual);
        _bondService.Verify(q => q.GetAccountsOverview(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _setService.Verify(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _fundService.Verify(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _geService.Verify(q => q.GetAccounts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _tfexService.Verify(q => q.GetPortfolioAccount(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _structureNoteService.Verify(
            q => q.GetStructureNotesPortfolioAccount(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Not_FetchAnything_When_FetchTradingAccountsIsEnabled_And_TradingAccountsWasEmpty()
    {
        // Arrange
        _featureService.Setup(q => q.IsOff(It.IsAny<string>()))
            .Returns(false);
        _featureService.Setup(q => q.IsOn(It.IsAny<string>()))
            .Returns(true);
        _userService.Setup(q => q.GetTradingAccountsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _bondService.Setup(q => q.GetAccountsOverview(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _setService.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _fundService.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _geService.Setup(q => q.GetAccounts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _tfexService.Setup(q => q.GetPortfolioAccount(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _structureNoteService.Setup(q =>
                q.GetStructureNotesPortfolioAccount(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var actual =
            await _portfolioManager.GetPortfolioSummaryV2Async(Guid.NewGuid().ToString(), "someValue",
                CancellationToken.None);

        // Assert
        Assert.IsType<PortfolioSummary>(actual);
        _bondService.Verify(q => q.GetAccountsOverview(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _setService.Verify(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _fundService.Verify(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _geService.Verify(q => q.GetAccounts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _tfexService.Verify(q => q.GetPortfolioAccount(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _structureNoteService.Verify(
            q => q.GetStructureNotesPortfolioAccount(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_FetchAll_When_FetchTradingAccountsIsDisabled()
    {
        // Arrange
        _featureService.Setup(q => q.IsOff(It.IsAny<string>()))
            .Returns(true);
        _featureService.Setup(q => q.IsOn(It.IsAny<string>()))
            .Returns(true);
        _userService.Setup(q => q.GetTradingAccountsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _bondService.Setup(q => q.GetAccountsOverview(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _setService.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _fundService.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _geService.Setup(q => q.GetAccounts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _tfexService.Setup(q => q.GetPortfolioAccount(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _structureNoteService.Setup(q =>
                q.GetStructureNotesPortfolioAccount(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var actual =
            await _portfolioManager.GetPortfolioSummaryV2Async(Guid.NewGuid().ToString(), "someValue",
                CancellationToken.None);

        // Assert
        Assert.IsType<PortfolioSummary>(actual);
        _userService.Verify(q => q.GetTradingAccountsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _bondService.Verify(q => q.GetAccountsOverview(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _setService.Verify(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _fundService.Verify(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _geService.Verify(q => q.GetAccounts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _tfexService.Verify(q => q.GetPortfolioAccount(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _structureNoteService.Verify(
            q => q.GetStructureNotesPortfolioAccount(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Not_FetchBond_When_BondFeatureFlagDisabled(bool fetchTradingAccountFlag)
    {
        // Arrange
        _featureService.Setup(q => q.IsOff(It.IsAny<string>()))
            .Returns(fetchTradingAccountFlag);
        _featureService.Setup(q => q.IsOn(It.IsAny<string>()))
            .Returns(false);
        _userService.Setup(q => q.GetTradingAccountsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                FakeTradingAccount(Product.Unknown),
                FakeTradingAccount(Product.Cash),
                FakeTradingAccount(Product.CashBalance),
                FakeTradingAccount(Product.CreditBalanceSbl),
                FakeTradingAccount(Product.Crypto),
                FakeTradingAccount(Product.Derivatives),
                FakeTradingAccount(Product.GlobalEquities),
                FakeTradingAccount(Product.Funds),
                FakeTradingAccount(Product.Bond),
                FakeTradingAccount(Product.CashSbl),
                FakeTradingAccount(Product.CashBalanceSbl),
                FakeTradingAccount(Product.CreditBalance),
                FakeTradingAccount(Product.StructureNoteOnShore),
                FakeTradingAccount(Product.Dr),
                FakeTradingAccount(Product.LiveX),
                FakeTradingAccount(Product.BorrowCash),
                FakeTradingAccount(Product.BorrowCashBalance)
            ]);
        _bondService.Setup(q => q.GetAccountsOverview(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _setService.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _fundService.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _geService.Setup(q => q.GetAccounts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _tfexService.Setup(q => q.GetPortfolioAccount(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _structureNoteService.Setup(q =>
                q.GetStructureNotesPortfolioAccount(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var actual =
            await _portfolioManager.GetPortfolioSummaryV2Async(Guid.NewGuid().ToString(), "someValue",
                CancellationToken.None);

        // Assert
        Assert.IsType<PortfolioSummary>(actual);
        _bondService.Verify(q => q.GetAccountsOverview(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _setService.Verify(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _fundService.Verify(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _geService.Verify(q => q.GetAccounts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _tfexService.Verify(q => q.GetPortfolioAccount(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _structureNoteService.Verify(
            q => q.GetStructureNotesPortfolioAccount(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [MemberData(nameof(ExpectedExecuteData))]
    public async Task Should_FetchAsExpected_When_FetchTradingAccountsIsEnabled_And_HadOnlyOneTradingAccount(Product product, Dictionary<Product, Times> expected)
    {
        // Arrange
        _featureService.Setup(q => q.IsOff(It.IsAny<string>()))
            .Returns(false);
        _featureService.Setup(q => q.IsOn(It.IsAny<string>()))
            .Returns(true);
        _userService.Setup(q => q.GetTradingAccountsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                FakeTradingAccount(product),
            ]);
        _bondService.Setup(q => q.GetAccountsOverview(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _setService.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _fundService.Setup(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _geService.Setup(q => q.GetAccounts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _tfexService.Setup(q => q.GetPortfolioAccount(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _structureNoteService.Setup(q =>
                q.GetStructureNotesPortfolioAccount(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var actual =
            await _portfolioManager.GetPortfolioSummaryV2Async(Guid.NewGuid().ToString(), "someValue",
                CancellationToken.None);

        // Assert
        Assert.IsType<PortfolioSummary>(actual);
        var setExpected = Times.Never();
        if (new[] { Product.Cash, Product.CashBalance, Product.CreditBalance, Product.Dr }.Contains(product))
        {
            setExpected = expected.TryGetValue(product, out var t2) ? t2 : Times.Never();
        }

        _setService.Verify(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), setExpected);
        _bondService.Verify(q => q.GetAccountsOverview(It.IsAny<string>(), It.IsAny<CancellationToken>()), expected.TryGetValue(Product.Bond, out var t1) ? t1 : Times.Never());
        _fundService.Verify(q => q.GetPortfolioAccounts(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), expected.TryGetValue(Product.Funds, out var t3) ? t3 : Times.Never());
        _geService.Verify(q => q.GetAccounts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), expected.TryGetValue(Product.GlobalEquities, out var t4) ? t4 : Times.Never());
        _tfexService.Verify(q => q.GetPortfolioAccount(It.IsAny<string>(), It.IsAny<CancellationToken>()), expected.TryGetValue(Product.Derivatives, out var t5) ? t5 : Times.Never());
        _structureNoteService.Verify(
            q => q.GetStructureNotesPortfolioAccount(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    public static IEnumerable<object[]> ExpectedExecuteData =>
        new List<object[]>
        {
            new object[] { Product.Cash, new Dictionary<Product, Times>()
            {
                { Product.Cash, Times.Once()},
                { Product.Bond, Times.Once()},
            }},
            new object[] { Product.CashBalance, new Dictionary<Product, Times>()
            {
                { Product.CashBalance, Times.Once()},
                { Product.Bond, Times.Once()},
            }},
            new object[] { Product.CreditBalance, new Dictionary<Product, Times>() { { Product.CreditBalance, Times.Once()} }},
            new object[] { Product.Crypto, new Dictionary<Product, Times>() { { Product.Crypto, Times.Once()} }},
            new object[] { Product.GlobalEquities, new Dictionary<Product, Times>() { { Product.GlobalEquities, Times.Once()} }},
            new object[] { Product.Funds, new Dictionary<Product, Times>() { { Product.Funds, Times.Once()} }},
            new object[] { Product.Bond, new Dictionary<Product, Times>() { { Product.Bond, Times.Once()} }},
            new object[] { Product.StructureNoteOnShore, new Dictionary<Product, Times>() { { Product.StructureNoteOnShore, Times.Once()} }},
            new object[] { Product.Dr, new Dictionary<Product, Times>() { { Product.Dr, Times.Once()} }},
        };

    public static TradingAccount FakeTradingAccount(Product product)
    {
        return new TradingAccount(Guid.NewGuid(), "0900433", "0900433-1", product);
    }
}
