using System.Text;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;
using Pi.SetMarketDataRealTime.Infrastructure.Services.SoupBinTcp.Handlers;

namespace Pi.SetMarketDataRealTime.Infrastructure.Tests;

public class HandlerTests
{
    private readonly Handler _handler;
    private readonly Mock<IClientListener> _mockClientListener;
    private readonly Mock<ILogger<Handler>> _mockLogger;

    public HandlerTests()
    {
        _mockClientListener = new Mock<IClientListener>();
        _mockLogger = new Mock<ILogger<Handler>>();
        var loginDetails = new LoginDetails { UserName = "testuser", Password = "testpass" };
        _handler = new Handler(loginDetails, _mockClientListener.Object, _mockLogger.Object, (bool isUnexpectedDisconnection) => { });
    }

    [Fact]
    public void ChannelRead_DebugMessage_InvokesOnDebug()
    {
        // Arrange
        var mockContext = new Mock<IChannelHandlerContext>();
        var debugMessage = new Debug("Debug message");

        // Act
        _handler.ChannelRead(mockContext.Object, debugMessage);

        // Assert
        _mockClientListener.Verify(x => x.OnDebug("Debug message"), Times.Once);
    }

    [Fact]
    public void ChannelRead_UnSequencedDataMessage_InvokesOnMessage()
    {
        // Arrange
        var mockContext = new Mock<IChannelHandlerContext>();
        var unSequencedData = new UnSequencedData(Encoding.ASCII.GetBytes("UnSequenced data"));

        // Act
        _handler.ChannelRead(mockContext.Object, unSequencedData);

        // Assert
        _mockClientListener.Verify(x => x.OnMessage(Encoding.ASCII.GetBytes("UnSequenced data")), Times.Once);
    }

    [Fact]
    public void ChannelRead_SequencedDataMessage_InvokesOnMessage()
    {
        // Arrange
        var mockContext = new Mock<IChannelHandlerContext>();
        var sequencedData = new SequencedData(Encoding.ASCII.GetBytes("Sequenced data"));

        // Act
        _handler.ChannelRead(mockContext.Object, sequencedData);

        // Assert
        _mockClientListener.Verify(x => x.OnMessage(Encoding.ASCII.GetBytes("Sequenced data")), Times.Once);
    }

    [Fact]
    public void ChannelRead_LoginAcceptedMessage_InvokesOnLoginAccept()
    {
        // Arrange
        var mockContext = new Mock<IChannelHandlerContext>();
        var loginAccepted = new LoginAccepted("session1", 123);

        // Act
        _handler.ChannelRead(mockContext.Object, loginAccepted);

        // Assert
        _mockClientListener.Verify(x => x.OnLoginAccept("session1  ", 123), Times.Once);
    }

    [Fact]
    public void ChannelRead_LoginRejectedMessage_InvokesOnLoginReject()
    {
        // Arrange
        var mockContext = new Mock<IChannelHandlerContext>();
        var loginRejected = new LoginRejected('A');

        // Act
        _handler.ChannelRead(mockContext.Object, loginRejected);

        // Assert
        _mockClientListener.Verify(x => x.OnLoginReject('A'), Times.Once);
    }
}