using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;
using Pi.SetMarketDataRealTime.Infrastructure.Services.SoupBinTcp.Handlers;

namespace Pi.SetMarketDataRealTime.Infrastructure.Tests
{
    public class HandlerTestsV2
    {
        private readonly Mock<IClientListener> _mockClientListener;
        private readonly Mock<ILogger<Handler>> _mockLogger;
        private readonly Mock<IChannelHandlerContext> _mockContext;
        private readonly LoginDetails _loginDetails;
        private readonly Handler _handler;

        public HandlerTestsV2()
        {
            _mockClientListener = new Mock<IClientListener>();
            _mockLogger = new Mock<ILogger<Handler>>();
            _mockContext = new Mock<IChannelHandlerContext>();
            _loginDetails = new LoginDetails { UserName = "testUser", Password = "testPass" };
            _handler = new Handler(_loginDetails, _mockClientListener.Object, _mockLogger.Object, (bool isUnexpectedDisconnection) => { });
        }

        [Fact]
        public void ChannelRead_Debug_CallsOnDebug()
        {
            // Arrange
            var debugMessage = new Debug("Test debug message");

            // Act
            _handler.ChannelRead(_mockContext.Object, debugMessage);

            // Assert
            _mockClientListener.Verify(cl => cl.OnDebug("Test debug message"), Times.Once);
        }

        [Fact]
        public void ChannelRead_UnSequencedData_CallsOnMessage()
        {
            // Arrange
            var unsequencedData = new UnSequencedData(new byte[] { 1, 2, 3 });

            // Act
            _handler.ChannelRead(_mockContext.Object, unsequencedData);

            // Assert
            _mockClientListener.Verify(cl => cl.OnMessage(unsequencedData.Message), Times.Once);
        }

        [Fact]
        public void ChannelRead_SequencedData_CallsOnMessage()
        {
            // Arrange
            var sequencedData = new SequencedData(new byte[] { 1, 2, 3 });

            // Act
            _handler.ChannelRead(_mockContext.Object, sequencedData);

            // Assert
            _mockClientListener.Verify(cl => cl.OnMessage(sequencedData.Message), Times.Once);
        }

        [Fact]
        public void ChannelRead_LoginAccepted_CallsOnLoginAccept()
        {
            // Arrange
            var loginAccepted = new LoginAccepted("testTest", 123);

            // Act
            _handler.ChannelRead(_mockContext.Object, loginAccepted);

            // Assert
            _mockClientListener.Verify(cl => cl.OnLoginAccept("testTest  ", 123), Times.Once);
        }

        [Fact]
        public void ChannelRead_LoginRejected_CallsOnLoginReject()
        {
            // Arrange
            var loginRejected = new LoginRejected('A');

            // Act
            _handler.ChannelRead(_mockContext.Object, loginRejected);

            // Assert
            _mockClientListener.Verify(cl => cl.OnLoginReject('A'), Times.Once);
        }

        [Fact]
        public void ChannelActive_ValidLoginDetails_SendsLoginRequest()
        {
            // Act
            _handler.ChannelActive(_mockContext.Object);

            // Assert
            _mockContext.Verify(c => c.WriteAndFlushAsync(It.IsAny<LoginRequest>()), Times.Once);
            _mockClientListener.Verify(cl => cl.OnConnect(), Times.Once);
        }

        [Fact]
        public void ChannelInactive_CallsOnUnexpectedDisconnection()
        {
            // Arrange
            bool onUnexpectedDisconnectionCalled = false;
            var handler = new Handler(_loginDetails, _mockClientListener.Object, _mockLogger.Object, (bool isUnexpectedDisconnection) => { onUnexpectedDisconnectionCalled = true; });

            // Act
            handler.ChannelInactive(_mockContext.Object);

            // Assert
            Assert.True(onUnexpectedDisconnectionCalled);
        }
    }
}