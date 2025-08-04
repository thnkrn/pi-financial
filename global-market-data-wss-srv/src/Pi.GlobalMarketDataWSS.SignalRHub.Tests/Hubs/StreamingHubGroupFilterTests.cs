using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.GlobalMarketDataWSS.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataWSS.Domain.Models.Request;
using Pi.GlobalMarketDataWSS.SignalRHub.Hubs;
using Pi.GlobalMarketDataWSS.SignalRHub.Interfaces;
using Moq;

namespace Pi.GlobalMarketDataWSS.SignalRHub.Tests.Hubs
{
    public class StreamingHubGroupFilterTests
    {
        private readonly Mock<IStreamingMarketDataSubscriberGroupFilter> _mockSubscriber;
        private readonly Mock<ILogger<StreamingHubGroupFilter>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IHubCallerClients> _mockClients;
        private readonly Mock<ISingleClientProxy> _mockClientProxy;
        private readonly HubCallerContext _mockHubCallerContext;
        private const string TestMethodName = "ReceiveMarketData";

        public StreamingHubGroupFilterTests()
        {
            _mockSubscriber = new Mock<IStreamingMarketDataSubscriberGroupFilter>();
            _mockLogger = new Mock<ILogger<StreamingHubGroupFilter>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockClients = new Mock<IHubCallerClients>();
            _mockClientProxy = new Mock<ISingleClientProxy>();
            _mockHubCallerContext = new Mock<HubCallerContext>().Object;

            _mockConfiguration.Setup(c => c[ConfigurationKeys.SignalRHubMethodName]).Returns(TestMethodName);
            _mockClients.Setup(c => c.Caller).Returns(_mockClientProxy.Object);
        }

        [Fact]
        public async Task SubscribeToStreamDataAsync_ValidRequest_SubscribesSuccessfully()
        {
            // Arrange
            var hub = new StreamingHubGroupFilter(_mockSubscriber.Object, _mockLogger.Object, _mockConfiguration.Object)
            {
                Clients = _mockClients.Object,
                Context = _mockHubCallerContext
            };
            var request = new MarketStreamingRequest();

            // Act
            await hub.SubscribeToStreamDataAsync(request);

            // Assert
            _mockSubscriber.Verify(s => s.UpdateSubscriptionAsync(It.IsAny<string>(), request), Times.Once);
            _mockClientProxy.Verify(
                c => c.SendCoreAsync(
                    TestMethodName,
                    It.Is<object[]>(args => args.Length == 1 && IsMessageWithContent(args[0], "Message", "Subscribed successfully")),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SubscribeToStreamDataAsync_ExceptionThrown_SendsErrorMessage()
        {
            // Arrange
            _mockSubscriber.Setup(s => s.UpdateSubscriptionAsync(It.IsAny<string>(), It.IsAny<MarketStreamingRequest>()))
                .ThrowsAsync(new Exception("Test exception"));

            var hub = new StreamingHubGroupFilter(_mockSubscriber.Object, _mockLogger.Object, _mockConfiguration.Object)
            {
                Clients = _mockClients.Object,
                Context = _mockHubCallerContext
            };
            var request = new MarketStreamingRequest();

            // Act
            await hub.SubscribeToStreamDataAsync(request);

            // Assert
            _mockClientProxy.Verify(
                c => c.SendCoreAsync(
                    TestMethodName,
                    It.Is<object[]>(args => args.Length == 1 && IsMessageWithContent(args[0], "Error", "Subscription failed")),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task OnDisconnectedAsync_RemovesSubscription()
        {
            // Arrange
            var hub = new StreamingHubGroupFilter(_mockSubscriber.Object, _mockLogger.Object, _mockConfiguration.Object)
            {
                Context = _mockHubCallerContext
            };

            // Act
            await hub.OnDisconnectedAsync(null);

            // Assert
            _mockSubscriber.Verify(s => s.RemoveSubscriptionAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DisposeAsync_DisposesSubscriberIfImplementsIAsyncDisposable()
        {
            // Arrange
            var mockAsyncDisposableSubscriber = new Mock<IStreamingMarketDataSubscriberGroupFilter>();
            mockAsyncDisposableSubscriber.As<IAsyncDisposable>();
            var hub = new StreamingHubGroupFilter(mockAsyncDisposableSubscriber.Object, _mockLogger.Object, _mockConfiguration.Object);

            // Act
            await hub.DisposeAsync();

            // Assert
            mockAsyncDisposableSubscriber.As<IAsyncDisposable>().Verify(d => d.DisposeAsync(), Times.Once);
        }

        private bool IsMessageWithContent(object? message, string propertyName, string expectedContent)
        {
            if (message == null) return false;
            var property = message.GetType().GetProperty(propertyName);
            if (property == null) return false;
            var value = property.GetValue(message)?.ToString();
            return value == expectedContent;
        }
    }
}