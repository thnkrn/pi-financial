using System.Reflection;
using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Pi.SetMarketData.MigrationProxy.Services;
using Pi.SetMarketData.MigrationProxy.ConstantConfigurations;
using Pi.SetMarketData.MigrationProxy.Models;
using Pi.SetMarketData.MigrationProxy.Interfaces;
using Pi.SetMarketData.MigrationProxy.Enums;

namespace Pi.SetMarketData.MigrationProxy.Tests;

public class ProxyServiceTests
{
    private readonly Mock<ILogger<ProxyService>> _loggerMock;
    private readonly Mock<IFeatureFlagService> _featureFlagServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly ProxyService _proxyService;
    private readonly string _SEThubUrl = "http://example.com/set";
    private readonly string _GEhubUrl = "http://example.com/ge";
    private readonly string _websocketUrl = "ws://example.com/socket";

    public ProxyServiceTests()
    {
        _loggerMock = new Mock<ILogger<ProxyService>>();
        _featureFlagServiceMock = new Mock<IFeatureFlagService>();
        _configurationMock = new Mock<IConfiguration>();

        var setHubUrlSection = new Mock<IConfigurationSection>();
        setHubUrlSection.Setup(s => s.Value).Returns(_SEThubUrl);
        _configurationMock.Setup(c => c.GetSection(ConfigurationKeys.SETSignalRHubURL)).Returns(setHubUrlSection.Object);

        var geHubUrlSection = new Mock<IConfigurationSection>();
        geHubUrlSection.Setup(s => s.Value).Returns(_GEhubUrl);
        _configurationMock.Setup(c => c.GetSection(ConfigurationKeys.GESignalRHubURL)).Returns(geHubUrlSection.Object);

        var websocketUrlSection = new Mock<IConfigurationSection>();
        websocketUrlSection.Setup(s => s.Value).Returns(_websocketUrl);
        _configurationMock.Setup(c => c.GetSection(ConfigurationKeys.WebSocketURL)).Returns(websocketUrlSection.Object);

        _proxyService = new ProxyService(_loggerMock.Object, _featureFlagServiceMock.Object, _configurationMock.Object);
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
        var expectedUrl = _SEThubUrl;

        // Act
        var result = InvokePrivateGetUrlEndpoint(SignalRHubEndpoint.SET);

        // Assert
        Assert.Equal(expectedUrl, result);
    }

    [Fact]
    public void GetUrlEndpoint_ShouldReturnGEHubUrl_WhenEndpointIsGE()
    {
        // Arrange
        var expectedUrl = _GEhubUrl;

        // Act
        var result = InvokePrivateGetUrlEndpoint(SignalRHubEndpoint.GE);

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
    public void DetermineSubscriptionList_ShouldSeparateSymbolsCorrectly()
    {
        // Arrange
        _featureFlagServiceMock.Setup(s => s.IsSETWebsocketProxyEnabled()).Returns(true);
        _featureFlagServiceMock.Setup(s => s.IsGEWebsocketProxyEnabled()).Returns(true);
        _featureFlagServiceMock.Setup(s => s.IsTFEXWebsocketProxyEnabled()).Returns(true);
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

    [Fact]
    public void DetermineSubscriptionList_ShouldHandleEmptyList()
    {
        // Arrange
        var payload = new RequestPayload();

        // Act
        var (SETPayload, GEPayload, wsPayload) = _proxyService.DetermineSubscriptionList(payload);

        // Assert
        Assert.Null(SETPayload);
        Assert.Null(GEPayload);
        Assert.Null(wsPayload);
    }

    [Fact]
    public void Constructor_ThrowsException_WhenHubUrlIsMissing()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        var emptySection = new Mock<IConfigurationSection>();
        emptySection.Setup(s => s.Value).Returns((string?)null);
        configMock.Setup(c => c.GetSection(ConfigurationKeys.SETSignalRHubURL)).Returns(emptySection.Object);

        var websocketSection = new Mock<IConfigurationSection>();
        websocketSection.Setup(s => s.Value).Returns(_websocketUrl);
        configMock.Setup(c => c.GetSection(ConfigurationKeys.WebSocketURL)).Returns(websocketSection.Object);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new ProxyService(_loggerMock.Object, _featureFlagServiceMock.Object, configMock.Object));
    }

    [Fact]
    public void Constructor_ThrowsException_WhenWebSocketUrlIsMissing()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        var emptySection = new Mock<IConfigurationSection>();
        emptySection.Setup(s => s.Value).Returns((string?)null);

        var hubSection = new Mock<IConfigurationSection>();
        hubSection.Setup(s => s.Value).Returns(_SEThubUrl);
        configMock.Setup(c => c.GetSection(ConfigurationKeys.SETSignalRHubURL)).Returns(hubSection.Object);
        configMock.Setup(c => c.GetSection(ConfigurationKeys.GESignalRHubURL)).Returns(hubSection.Object);

        configMock.Setup(c => c.GetSection(ConfigurationKeys.WebSocketURL)).Returns(emptySection.Object);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new ProxyService(_loggerMock.Object, _featureFlagServiceMock.Object, configMock.Object));
    }

    [Fact]
    public async Task SubscribeSignalRHub_ShouldNotInvokeWhenSymbolsEmpty()
    {
        // Arrange
        var webSocketMock = new Mock<WebSocket>();

        // Act
        await _proxyService.SubscribeSignalRHub(webSocketMock.Object, null, SignalRHubEndpoint.SET);

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
        await _proxyService.SubscribeWebsocket(webSocketMock.Object, null);

        // Assert
        _loggerMock.Verify(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Never);
    }
}