using Moq;
using Pi.Client.BondApi.Api;
using Pi.Client.BondApi.Model;
using Pi.Client.OnePort.GW.DB2.Api;
using Pi.Client.OnePort.GW.DB2.Client;
using Pi.Client.OnePort.GW.DB2.Model;
using Pi.Common.Features;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Infrastructure.Services;
using IDb2TradingApi = Pi.Client.OnePort.GW.DB2.Api.ITradingApi;
using ITcpTradingApi = Pi.Client.OnePort.GW.TCP.Api.ITradingApi;

namespace Pi.SetService.Infrastructure.Tests.Services;

public class OnePortServiceTest
{
    private readonly Mock<IAccountApi> _accountApi;
    private readonly Mock<IMarketDataApi> _bondMarketDataApi;
    private readonly Mock<IDb2TradingApi> _db2Api;
    private readonly Mock<IFeatureService> _featureService;
    private readonly OnePortService _onePortService;
    private readonly Mock<ITcpTradingApi> _tcpApi;
    private readonly string BondSymbol = "TRUE294A";

    public OnePortServiceTest()
    {
        _db2Api = new Mock<IDb2TradingApi>();
        _tcpApi = new Mock<ITcpTradingApi>();
        _accountApi = new Mock<IAccountApi>();
        _bondMarketDataApi = new Mock<IMarketDataApi>();
        _featureService = new Mock<IFeatureService>();
        _onePortService = new OnePortService(_db2Api.Object, _tcpApi.Object, _accountApi.Object,
            _bondMarketDataApi.Object, _featureService.Object);

        _bondMarketDataApi.Setup(q => q.InternalBondsSymbolsGetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InternalBondsSymbolsGet200Response
            {
                Data = new InternalDomainMarketdataHandlerBondResponse { Symbols = [BondSymbol] }
            });
        _featureService.Setup(q => q.IsOff(It.IsAny<string>()))
            .Returns(false);
    }

    [Fact]
    public async Task Should_Return_ExpectedPositions_When_GetPositions_Success()
    {
        // Arrange
        var tradingAccountNo = "0900111-8";
        var positions = new List<PiOnePortDb2ModelsAccountPosition>
        {
            new(tradingAccountNo, "EA", null, "", "", 100, 100, 100, 100, 1, 100, 100),
            new(tradingAccountNo, "BANPU", null, "", "", 100, 100, 100, 100, 1, 100, 100),
            new(tradingAccountNo, BondSymbol, null, "", "", 100, 100, 100, 100, 1, 100, 100)
        };
        _accountApi.Setup(q =>
                q.GetAccountPositionAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PiOnePortDb2ModelsAccountPositionListApiResponse { Data = positions });

        // Act
        var actual = await _onePortService.GetPositions(tradingAccountNo, CancellationToken.None);

        // Assert
        Assert.IsType<List<AccountPosition>>(actual);
        Assert.Equal(["EA", "BANPU"], actual.Select(q => q.SecSymbol));
    }

    [Fact]
    public async Task Should_Return_ExpectedPositions_When_GetPositions_Success_And_FeatureFlagOff()
    {
        // Arrange
        var tradingAccountNo = "0900111-8";
        var positions = new List<PiOnePortDb2ModelsAccountPosition>
        {
            new(tradingAccountNo, "EA", null, "", "", 100, 100, 100, 100, 1, 100, 100),
            new(tradingAccountNo, "BANPU", null, "", "", 100, 100, 100, 100, 1, 100, 100),
            new(tradingAccountNo, BondSymbol, null, "", "", 100, 100, 100, 100, 1, 100, 100)
        };
        _accountApi.Setup(q =>
                q.GetAccountPositionAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PiOnePortDb2ModelsAccountPositionListApiResponse { Data = positions });
        _featureService.Setup(q => q.IsOff(It.IsAny<string>()))
            .Returns(true);

        // Act
        var actual = await _onePortService.GetPositions(tradingAccountNo, CancellationToken.None);

        // Assert
        Assert.IsType<List<AccountPosition>>(actual);
        Assert.Equal(["EA", "BANPU", BondSymbol], actual.Select(q => q.SecSymbol));
    }

    [Fact]
    public async Task Should_Return_Empty_When_GetPositions_And_PositionNotFound()
    {
        // Arrange
        var tradingAccountNo = "0900111-8";
        _accountApi.Setup(q =>
                q.GetAccountPositionAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PiOnePortDb2ModelsAccountPositionListApiResponse { Data = [] });

        // Act
        var actual = await _onePortService.GetPositions(tradingAccountNo, CancellationToken.None);

        // Assert
        Assert.IsType<List<AccountPosition>>(actual);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task Should_Return_Empty_When_GetPositions_And_GotApiException()
    {
        // Arrange
        var tradingAccountNo = "0900111-8";
        _accountApi.Setup(q =>
                q.GetAccountPositionAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .Throws(new ApiException());

        // Act
        var actual = await _onePortService.GetPositions(tradingAccountNo, CancellationToken.None);

        // Assert
        Assert.IsType<List<AccountPosition>>(actual);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task Should_Error_When_GetPositions_And_GotError()
    {
        // Arrange
        var tradingAccountNo = "0900111-8";
        _accountApi.Setup(q =>
                q.GetAccountPositionAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception());

        // Act
        var act = async () => await _onePortService.GetPositions(tradingAccountNo, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<Exception>(act);
    }

    [Fact]
    public async Task Should_Return_ExpectedPositions_When_GetPositionsCreditBalance_Success()
    {
        // Arrange
        var tradingAccountNo = "0900111-8";
        var positions = new List<PiOnePortDb2ModelsAccountPositionCreditBalance>
        {
            new(tradingAccountNo, "EA", null, "", "", 100, 100, 100, 100, 1, 100, 100)
                { DelFlag = "N", UpdateFlag = "N" },
            new(tradingAccountNo, "BANPU", null, "", "", 100, 100, 100, 100, 1, 100, 100)
                { DelFlag = "N", UpdateFlag = "N" },
            new(tradingAccountNo, BondSymbol, null, "", "", 100, 100, 100, 100, 1, 100, 100)
                { DelFlag = "N", UpdateFlag = "N" }
        };
        _accountApi.Setup(q =>
                q.GetAccountCreditBalancePositionAsync(It.IsAny<string>(), It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PiOnePortDb2ModelsAccountPositionCreditBalanceListApiResponse { Data = positions });

        // Act
        var actual = await _onePortService.GetPositionsCreditBalance(tradingAccountNo, CancellationToken.None);

        // Assert
        Assert.IsType<List<AccountPositionCreditBalance>>(actual);
        Assert.Equal(["EA", "BANPU"], actual.Select(q => q.SecSymbol));
    }

    [Fact]
    public async Task Should_Return_Empty_When_GetPositionsCreditBalance_And_PositionNotFound()
    {
        // Arrange
        var tradingAccountNo = "0900111-8";
        _accountApi.Setup(q =>
                q.GetAccountCreditBalancePositionAsync(It.IsAny<string>(), It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PiOnePortDb2ModelsAccountPositionCreditBalanceListApiResponse { Data = [] });

        // Act
        var actual = await _onePortService.GetPositionsCreditBalance(tradingAccountNo, CancellationToken.None);

        // Assert
        Assert.IsType<List<AccountPositionCreditBalance>>(actual);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task Should_Error_When_GetAccountCreditBalancePositionAsync_And_GotError()
    {
        // Arrange
        var tradingAccountNo = "0900111-8";
        _accountApi.Setup(q =>
                q.GetAccountCreditBalancePositionAsync(It.IsAny<string>(), It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
            .Throws(new Exception());

        // Act
        var act = async () => await _onePortService.GetPositionsCreditBalance(tradingAccountNo, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<Exception>(act);
    }
}