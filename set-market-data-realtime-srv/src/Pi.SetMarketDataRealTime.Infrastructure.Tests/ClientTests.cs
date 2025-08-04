using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Services.SoupBinTcp;

namespace Pi.SetMarketDataRealTime.Infrastructure.Tests
{
    public class ClientTests
    {
        private readonly Mock<IClientListener> _mockClientListener;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;
        private readonly Mock<ILogger<Client>> _mockClientLogger;
        private readonly Client _client;

        public ClientTests()
        {
            _mockClientListener = new Mock<IClientListener>();
            _mockLoggerFactory = new Mock<ILoggerFactory>();
            _mockClientLogger = new Mock<ILogger<Client>>();

            _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_mockClientLogger.Object);

            _client = new Client(_mockClientListener.Object, _mockLoggerFactory.Object);
        }

        [Fact]
        public async Task SetupAsync_ValidParameters_SetsPropertiesCorrectly()
        {
            // Arrange
            var ipAddress = IPAddress.Loopback;
            const int port = 1234;
            const int reconnectDelayMs = 5000;
            var loginDetails = new LoginDetails { UserName = "testUser", Password = "testPass" };

            // Act
            await _client.SetupAsync(ipAddress, port, reconnectDelayMs, loginDetails);

            // Assert
            Assert.Equal(ClientState.Disconnected, _client.State);
        }

        [Fact]
        public async Task StartAsync_DisconnectedState_ChangesStateToConnecting()
        {
            // Arrange
            await SetupClientAsync();

            // Act
            _ = _client.StartAsync();

            // Assert
            Assert.Equal(ClientState.Connecting, _client.State);

            // Cleanup
            await _client.ShutdownAsync();
        }

        private async Task SetupClientAsync()
        {
            var ipAddress = IPAddress.Loopback;
            const int port = 1234;
            const int reconnectDelayMs = 5000;
            var loginDetails = new LoginDetails { UserName = "testUser", Password = "testPass" };
            await _client.SetupAsync(ipAddress, port, reconnectDelayMs, loginDetails);
        }
    }
}