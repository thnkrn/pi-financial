using System.Net.WebSockets;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.MarketData.Application.Interfaces;
using Pi.MarketData.Application.Services;
using Pi.MarketData.Domain.ConstantConfigurations;
using Pi.MarketData.Domain.Enums;
using Pi.MarketData.Domain.Models;

namespace Pi.MarketData.MigrationProxy.Tests.Service;

public class ProxyServiceTests
{
    private readonly Mock<IFeatureFlagService> _featureFlagServiceMock;
    private readonly Mock<ILogger<ProxyService>> _loggerMock;
    private readonly ProxyService _proxyService;
    private readonly string _setHubUrl = "http://example.com/hub";
    private readonly string _geHubUrl = "http://example.com/ge";
    private readonly string _websocketUrl = "ws://example.com/socket";

    public ProxyServiceTests()
    {
        _loggerMock = new Mock<ILogger<ProxyService>>();
        _featureFlagServiceMock = new Mock<IFeatureFlagService>();
        Mock<IConfiguration> configurationMock = new();

        var hubUrlSection = new Mock<IConfigurationSection>();
        hubUrlSection.Setup(s => s.Value).Returns(_setHubUrl);
        configurationMock.Setup(c => c.GetSection(ConfigurationKeys.SetSignalRHubUrl)).Returns(hubUrlSection.Object);

        var geHubUrlSection = new Mock<IConfigurationSection>();
        geHubUrlSection.Setup(s => s.Value).Returns(_geHubUrl);
        configurationMock.Setup(c => c.GetSection(ConfigurationKeys.GeSignalRHubUrl)).Returns(geHubUrlSection.Object);

        var websocketUrlSection = new Mock<IConfigurationSection>();
        websocketUrlSection.Setup(s => s.Value).Returns(_websocketUrl);
        configurationMock.Setup(c => c.GetSection(ConfigurationKeys.WebSocketUrl)).Returns(websocketUrlSection.Object);

        var growthBookByPassIsActivated = new Mock<IConfigurationSection>();
        growthBookByPassIsActivated.Setup(s => s.Value).Returns("true");
        configurationMock.Setup(c => c.GetSection(ConfigurationKeys.GrowthBookByPassActivated))
            .Returns(growthBookByPassIsActivated.Object);

        var growthBookByPassSetFlag = new Mock<IConfigurationSection>();
        growthBookByPassSetFlag.Setup(s => s.Value).Returns("true");
        configurationMock.Setup(c => c.GetSection(ConfigurationKeys.GrowthBookByPassSetWebsocket))
            .Returns(growthBookByPassSetFlag.Object);

        var growthBookByPassGeFlag = new Mock<IConfigurationSection>();
        growthBookByPassGeFlag.Setup(s => s.Value).Returns("true");
        configurationMock.Setup(c => c.GetSection(ConfigurationKeys.GrowthBookByPassGeWebsocket))
            .Returns(growthBookByPassSetFlag.Object);

        var growthBookByPassTfexFlag = new Mock<IConfigurationSection>();
        growthBookByPassTfexFlag.Setup(s => s.Value).Returns("true");
        configurationMock.Setup(c => c.GetSection(ConfigurationKeys.GrowthBookByPassTfexWebsocket))
            .Returns(growthBookByPassSetFlag.Object);

        _proxyService = new ProxyService(_loggerMock.Object, _featureFlagServiceMock.Object, configurationMock.Object);
    }

    [Fact]
    public void DetermineSubscriptionList_ShouldSeparateSymbolsCorrectly()
    {
        // Arrange
        _featureFlagServiceMock.Setup(s => s.IsSetWebsocketProxyEnabled()).Returns(true);
        _featureFlagServiceMock.Setup(s => s.IsGeWebsocketProxyEnabled()).Returns(true);
        _featureFlagServiceMock.Setup(s => s.IsTfexWebsocketProxyEnabled()).Returns(true);
        var payload = new RequestPayload
        {
            Data = new Data
            {
                Param =
                [
                    new Parameter { Market = "Equity", Symbol = "SET" },
                    new Parameter { Market = "Equity", Symbol = "SET50" },
                    new Parameter { Market = "Equity", Symbol = "MAI" },
                    new Parameter { Market = "NASDAQ", Symbol = "NDX.INDEX" },
                    new Parameter { Market = "HKEX", Symbol = "HSI.INDEX" },
                    new Parameter { Market = "NYSE", Symbol = "SPX.INDEX" },
                    new Parameter { Market = "Commodity", Symbol = "GOLD$" },
                    new Parameter { Market = "Commodity", Symbol = "BRENT$" },
                    new Parameter { Market = "Currency", Symbol = "USD" },
                    new Parameter { Market = "Equity", Symbol = "KBANK" },
                    new Parameter { Market = "Equity", Symbol = "KTB" },
                    new Parameter { Market = "Equity", Symbol = "SCB" },
                    new Parameter { Market = "Equity", Symbol = "BBL" },
                    new Parameter { Market = "Equity", Symbol = "CPALL" }
                ],
                SubscribeType = "ticker/v2"
            },
            Op = "Subscribe",
            SessionId = "session-id"
        };

        // Act
        var (SETPayload, GEPayload, WsPayload) = _proxyService.DetermineSubscriptionList(payload);

        // Assert
        Assert.NotNull(SETPayload);
        Assert.NotNull(GEPayload);
        Assert.NotNull(WsPayload);

        // SET Payload assertions
        Assert.Equal(8, SETPayload!.Data!.Param!.Count);
        Assert.All(SETPayload.Data.Param, p => Assert.Equal("Equity", p.Market));

        // GE Payload assertions
        Assert.Empty(GEPayload!.Data!.Param!);

        // WebSocket Payload assertions
        Assert.Equal(6, WsPayload!.Data!.Param!.Count);
        Assert.Contains(WsPayload.Data.Param, p => p.Market == "NASDAQ" && p.Symbol == "NDX.INDEX");
        Assert.Contains(WsPayload.Data.Param, p => p.Market == "HKEX" && p.Symbol == "HSI.INDEX");
        Assert.Contains(WsPayload.Data.Param, p => p.Market == "NYSE" && p.Symbol == "SPX.INDEX");
        Assert.Contains(WsPayload.Data.Param, p => p.Market == "Commodity" && p.Symbol == "GOLD$");
        Assert.Contains(WsPayload.Data.Param, p => p.Market == "Commodity" && p.Symbol == "BRENT$");
        Assert.Contains(WsPayload.Data.Param, p => p.Market == "Currency" && p.Symbol == "USD");

        // Common assertions
        Assert.All(new[] { SETPayload, GEPayload, WsPayload }, p =>
        {
            Assert.Equal("Subscribe", p.Op);
            Assert.Equal("session-id", p.SessionId);
            Assert.Equal("ticker/v2", p.Data!.SubscribeType);
        });
    }

    private string InvokePrivateGetUrlEndpoint(SignalRHubEndpoint endpoint)
    {
        var methodInfo = typeof(ProxyService).GetMethod("GetUrlEndpoint", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Could not find the 'GetUrlEndpoint' method.");
        var result = methodInfo.Invoke(_proxyService, [endpoint]);
        return result as string ?? throw new InvalidOperationException("The method returned a null value.");
    }

    [Fact]
    public void GetUrlEndpoint_ShouldReturnSETHubUrl_WhenEndpointIsSET()
    {
        // Arrange
        var expectedUrl = _setHubUrl;

        // Act
        var result = InvokePrivateGetUrlEndpoint(SignalRHubEndpoint.Set);

        // Assert
        Assert.Equal(expectedUrl, result);
    }

    [Fact]
    public void GetUrlEndpoint_ShouldReturnGEHubUrl_WhenEndpointIsGE()
    {
        // Arrange
        var expectedUrl = _geHubUrl;

        // Act
        var result = InvokePrivateGetUrlEndpoint(SignalRHubEndpoint.Ge);

        // Assert
        Assert.Equal(expectedUrl, result);
    }

    [Fact]
    public void GetUrlEndpoint_ShouldThrowInvalidOperationException_WhenEndpointIsInvalid()
    {
        // Arrange
        var invalidEndpoint = (SignalRHubEndpoint)999;

        // Act
        var exception = Assert.Throws<TargetInvocationException>(() =>
            InvokePrivateGetUrlEndpoint(invalidEndpoint));

        // Assert
        var innerException = exception.InnerException;
        Assert.IsType<InvalidOperationException>(innerException);
        Assert.Equal("Endpoint value was 999. No valid return value provided.", innerException.Message);
    }

    [Fact]
    public void DetermineSubscriptionList_ShouldHandleEmptyList()
    {
        // Arrange
        var payload = new RequestPayload();

        // Act
        var (setPayload, gePayload, wsPayload) = _proxyService.DetermineSubscriptionList(payload);

        // Assert
        Assert.Null(setPayload);
        Assert.Null(gePayload);
        Assert.Null(wsPayload);
    }

    [Fact]
    public void Constructor_ThrowsException_WhenHubUrlIsMissing()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        var emptySection = new Mock<IConfigurationSection>();
        emptySection.Setup(s => s.Value).Returns((string?)null);
        configMock.Setup(c => c.GetSection(ConfigurationKeys.SetSignalRHubUrl)).Returns(emptySection.Object);

        var websocketSection = new Mock<IConfigurationSection>();
        websocketSection.Setup(s => s.Value).Returns(_websocketUrl);
        configMock.Setup(c => c.GetSection(ConfigurationKeys.WebSocketUrl)).Returns(websocketSection.Object);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            new ProxyService(_loggerMock.Object, _featureFlagServiceMock.Object, configMock.Object));
    }

    [Fact]
    public void Constructor_ThrowsException_WhenWebSocketUrlIsMissing()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        var emptySection = new Mock<IConfigurationSection>();
        emptySection.Setup(s => s.Value).Returns((string?)null);

        var hubSection = new Mock<IConfigurationSection>();
        hubSection.Setup(s => s.Value).Returns(_setHubUrl);
        configMock.Setup(c => c.GetSection(ConfigurationKeys.SetSignalRHubUrl)).Returns(hubSection.Object);
        configMock.Setup(c => c.GetSection(ConfigurationKeys.GeSignalRHubUrl)).Returns(hubSection.Object);

        configMock.Setup(c => c.GetSection(ConfigurationKeys.WebSocketUrl)).Returns(emptySection.Object);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            new ProxyService(_loggerMock.Object, _featureFlagServiceMock.Object, configMock.Object));
    }

    [Fact]
    public async Task SubscribeSignalRHub_ShouldNotInvokeWhenSymbolsEmpty()
    {
        // Arrange
        var webSocketMock = new Mock<WebSocket>();

        // Act
        await _proxyService.SubscribeSignalRHub(webSocketMock.Object, null, SignalRHubEndpoint.Set,
            new CancellationToken());

        // Assert
        _loggerMock.Verify(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Never);
    }

    [Fact]
    public async Task SubscribeWebsocket_ShouldNotConnectWhenPayloadIsNull()
    {
        // Arrange
        var webSocketMock = new Mock<WebSocket>();

        // Act
        await _proxyService.SubscribeWebsocket(webSocketMock.Object, null, new CancellationToken());

        // Assert
        _loggerMock.Verify(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Never);
    }
}