using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.MarketData.Application.Interfaces;
using Pi.MarketData.Application.Services;
using Pi.MarketData.Domain.Models;

namespace Pi.MarketData.MigrationProxy.Tests.Service;

public class WebSocketServiceTests
{
    private readonly Mock<ILogger<WebSocketService>> _loggerMock;
    private readonly Mock<IProxyService> _proxyServiceMock;
    private readonly WebSocketService _service;

    public WebSocketServiceTests()
    {
        _loggerMock = new Mock<ILogger<WebSocketService>>();
        _proxyServiceMock = new Mock<IProxyService>();
        _service = new WebSocketService(_loggerMock.Object, _proxyServiceMock.Object);
    }

    [Fact]
    public void AddClient_ShouldAddClientToCollection()
    {
        // Arrange
        var webSocket = new Mock<WebSocket>().Object;

        // Act
        _service.AddClient(webSocket);

        // Assert
        Assert.Single(_service.GetAllClients());
        Assert.Contains(webSocket, _service.GetAllClients().Keys);
    }

    [Fact]
    public void RemoveClient_ShouldRemoveClientFromCollection()
    {
        // Arrange
        var webSocket = new Mock<WebSocket>().Object;
        _service.AddClient(webSocket);

        // Act
        _service.RemoveClient(webSocket);

        // Assert
        Assert.Empty(_service.GetAllClients());
    }

    [Fact]
    public async Task SendMessageToClientAsync_ShouldSendMessageWhenSocketIsOpen()
    {
        // Arrange
        var webSocketMock = new Mock<WebSocket>();
        webSocketMock.Setup(w => w.State).Returns(WebSocketState.Open);
        _service.AddClient(webSocketMock.Object);

        var message = "Test message";

        // Act
        await _service.SendMessageToClientAsync(webSocketMock.Object, message);

        // Assert
        webSocketMock.Verify(w =>
            w.SendAsync(It.Is<ArraySegment<byte>>(b => 
                b.Array != null && Encoding.UTF8.GetString(b.Array, b.Offset, b.Count) == message),
                WebSocketMessageType.Text, 
                true, 
                It.IsAny<CancellationToken>()), 
            Times.Once);

    }

    [Fact]
    public async Task CloseAllConnections_ShouldCloseAllOpenConnections()
    {
        // Arrange
        var webSocket1 = new Mock<WebSocket>();
        var webSocket2 = new Mock<WebSocket>();
        webSocket1.Setup(w => w.State).Returns(WebSocketState.Open);
        webSocket2.Setup(w => w.State).Returns(WebSocketState.Open);

        _service.AddClient(webSocket1.Object);
        _service.AddClient(webSocket2.Object);

        // Act
        await _service.CloseAllConnections();

        // Assert
        webSocket1.Verify(
            w => w.CloseAsync(WebSocketCloseStatus.NormalClosure, It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
        webSocket2.Verify(
            w => w.CloseAsync(WebSocketCloseStatus.NormalClosure, It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void ParseMessage_ShouldReturnRequestPayload_WhenValidJson()
    {
        // Arrange
        var payload = new RequestPayload { Op = "Subscribe" };
        var message = JsonConvert.SerializeObject(payload);

        // Act
        var result = _service.ParseMessage(message);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(payload.Op, result.Op);
    }

    [Fact]
    public void ParseMessage_ShouldReturnNull_WhenInvalidJson()
    {
        // Arrange
        var invalidMessage = "Invalid JSON";

        // Act
        var result = _service.ParseMessage(invalidMessage);

        // Assert
        Assert.Null(result);
        _loggerMock.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error parsing message")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Dispose_ShouldCancelAndDisposeAllClients()
    {
        // Arrange
        var webSocket1 = new Mock<WebSocket>().Object;
        var webSocket2 = new Mock<WebSocket>().Object;

        _service.AddClient(webSocket1);
        _service.AddClient(webSocket2);

        // Act
        _service.Dispose();

        // Assert
        Assert.Empty(_service.GetAllClients());
    }
}