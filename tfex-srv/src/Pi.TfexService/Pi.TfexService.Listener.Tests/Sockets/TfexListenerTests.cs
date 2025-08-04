using System.Reflection;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Google.Type;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using Pi.Financial.Client.SetTradeOms.Model;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Infrastructure.Options;
using Pi.TfexService.Listener.Sockets;
using Pi.TfexService.Listener.Models;

namespace Pi.TfexService.Listener.Tests.Sockets;

public class TfexListenerTests
{
    private readonly Mock<ILogger<TfexListener>> _mockLogger;
    private readonly Mock<IBus> _mockBus;
    private readonly Mock<IHostEnvironment> _mockEnvironment;
    private readonly Mock<IServiceScopeFactory> _mockServiceScopeFactory;
    private readonly Mock<ISetTradeService> _mockSetTradeService;
    private readonly Mock<IMqttClientFactory> _mockMqttClientFactory;
    private readonly Mock<IOptions<FeaturesOptions>> _mockFeaturesOptions;
    private readonly Mock<IOptions<SetTradeStreamOptions>> _mockSetTradeStreamOptions;
    private readonly Mock<IOptions<OperationHoursOptions>> _mockOperationHoursOptions;

    public TfexListenerTests()
    {
        _mockLogger = new Mock<ILogger<TfexListener>>();
        _mockBus = new Mock<IBus>();
        _mockEnvironment = new Mock<IHostEnvironment>();
        _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        _mockSetTradeService = new Mock<ISetTradeService>();
        _mockMqttClientFactory = new Mock<IMqttClientFactory>();
        _mockSetTradeStreamOptions = new Mock<IOptions<SetTradeStreamOptions>>();
        _mockOperationHoursOptions = new Mock<IOptions<OperationHoursOptions>>();
        _mockFeaturesOptions = new Mock<IOptions<FeaturesOptions>>();

        var mockServiceScope = new Mock<IServiceScope>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockMqttClient = new Mock<IMqttClient>();

        mockServiceScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(ISetTradeService))).Returns(_mockSetTradeService.Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IMqttClientFactory))).Returns(_mockMqttClientFactory.Object);

        _mockEnvironment.Setup(env => env.EnvironmentName).Returns(Environments.Development);
        _mockServiceScopeFactory.Setup(x => x.CreateScope()).Returns(mockServiceScope.Object);
        _mockMqttClientFactory.Setup(x => x.CreateMqttClient()).Returns(mockMqttClient.Object);

        _mockSetTradeStreamOptions.Setup(x => x.Value).Returns(new SetTradeStreamOptions
        {
            Topic = ["test/topic"],
            Path = "/mqtt",
            ConnectionDelaySeconds = 5,
            HealthCheckSeconds = 1
        });

        _mockOperationHoursOptions.Setup(x => x.Value).Returns(new OperationHoursOptions
        {
            Start = "00:00:00",
            End = "23:59:59"
        });

        _mockFeaturesOptions.Setup(x => x.Value).Returns(new FeaturesOptions
        {
            IsTfexListenerNotificationEnabled = true
        });
    }

    private TfexListener CreateTfexListener()
    {
        return new TfexListener(
            _mockLogger.Object,
            _mockBus.Object,
            _mockEnvironment.Object,
            _mockServiceScopeFactory.Object,
            _mockSetTradeStreamOptions.Object,
            _mockOperationHoursOptions.Object,
            _mockFeaturesOptions.Object);
    }

    [Fact]
    public async Task Should_Connect_Listener()
    {
        // Arrange
        var listener = CreateTfexListener();
        var tokenSource = new CancellationTokenSource();
        var cancellationToken = tokenSource.Token;
        listener.HealthCheckCancellationTokenSource = tokenSource;

        _mockSetTradeService.Setup(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SettradeStreamResponse
            {
                Hosts = ["wss://test.mqtt"],
                Token = "test-token"
            });

        // Act
        await listener.StartAsync(cancellationToken);

        // Assert
        _mockMqttClientFactory.Verify(x => x.Connect(It.IsAny<IMqttClient>(), It.IsAny<MqttClientOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Stop_Listener_When_Cancel()
    {
        // Arrange
        var listener = CreateTfexListener();
        var tokenSource = new CancellationTokenSource();
        var cancellationToken = tokenSource.Token;

        _mockSetTradeService.Setup(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SettradeStreamResponse
            {
                Hosts = ["wss://test.mqtt"],
                Token = "test-token"
            });

        // Act
        await tokenSource.CancelAsync();
        await listener.StartAsync(cancellationToken);

        // Assert
        _mockMqttClientFactory.Verify(x => x.Connect(It.IsAny<IMqttClient>(), It.IsAny<MqttClientOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Stop_Listener()
    {
        // Arrange
        var mockMqttClient = new Mock<IMqttClient>();
        mockMqttClient.Setup(x => x.IsConnected).Returns(true);
        _mockMqttClientFactory.Setup(x => x.CreateMqttClient()).Returns(mockMqttClient.Object);

        var listener = CreateTfexListener();
        var tokenSource = new CancellationTokenSource();
        var cancellationToken = tokenSource.Token;

        _mockSetTradeService.Setup(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SettradeStreamResponse
            {
                Hosts = ["wss://test.mqtt"],
                Token = "test-token"
            });

        // Act
        await listener.StartAsync(cancellationToken);
        await listener.StopAsync(cancellationToken);

        // Assert
        _mockMqttClientFactory.Verify(x => x.Disconnect(It.IsAny<IMqttClient>(), It.IsAny<MqttClientDisconnectOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Initialize_Config()
    {
        // Arrange
        var listener = CreateTfexListener();
        var cancellationToken = new CancellationTokenSource().Token;

        _mockSetTradeService.Setup(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SettradeStreamResponse
            {
                Hosts = ["wss://test.mqtt"],
                Token = "test-token"
            });

        // Act
        await listener.StartAsync(cancellationToken);

        // Assert
        _mockMqttClientFactory.Verify(x => x.CreateMqttClient(), Times.Once);
    }

    [Fact]
    public async Task Should_Create_Broker_Options()
    {
        // Arrange
        var listener = CreateTfexListener();
        var cancellationToken = new CancellationTokenSource().Token;

        _mockSetTradeService.Setup(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SettradeStreamResponse
            {
                Hosts = ["wss://test.mqtt"],
                Token = "test-token"
            });

        // Act
        await listener.StartAsync(cancellationToken);

        // Assert
        _mockMqttClientFactory.Verify(x => x.CreateBrokerOptions(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Should_Create_Subscribe_Options()
    {
        // Arrange
        var listener = CreateTfexListener();
        var cancellationToken = new CancellationTokenSource().Token;

        _mockSetTradeService.Setup(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SettradeStreamResponse
            {
                Hosts = ["wss://test.mqtt"],
                Token = "test-token"
            });

        // Act
        await listener.StartAsync(cancellationToken);

        // Assert
        _mockMqttClientFactory.Verify(x => x.CreateSubscribeOptions(It.IsAny<List<string>>()), Times.Once);
    }

    [Fact]
    public async Task Should_Retrieve_New_Access_Token_When_Connect_Failed_With_Authorization()
    {
        // Arrange
        var listener = CreateTfexListener();
        var tokenSource = new CancellationTokenSource();
        var cancellationToken = tokenSource.Token;

        _mockSetTradeService.Setup(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SettradeStreamResponse
            {
                Hosts = ["wss://test.mqtt"],
                Token = "test-token"
            });

        _mockMqttClientFactory
            .Setup(x => x.Connect(It.IsAny<IMqttClient>(), It.IsAny<MqttClientOptions>(),
                It.IsAny<CancellationToken>())).Throws(new Exception("401"));

        // Act
        await listener.StartAsync(cancellationToken);

        // Assert
        _mockSetTradeService.Verify(x => x.GetAccessToken(true), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Should_Retry_When_Connect_Failed_With_Not_Authorization()
    {
        // Arrange
        var listener = CreateTfexListener();
        var tokenSource = new CancellationTokenSource();
        var cancellationToken = tokenSource.Token;

        _mockSetTradeService.Setup(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SettradeStreamResponse
            {
                Hosts = ["wss://test.mqtt"],
                Token = "test-token"
            });

        _mockMqttClientFactory
            .Setup(x => x.Connect(It.IsAny<IMqttClient>(), It.IsAny<MqttClientOptions>(),
                It.IsAny<CancellationToken>())).Throws(new Exception("SetTrade Down"));

        // Act
        await listener.StartAsync(cancellationToken);

        // Assert
        _mockSetTradeService.Verify(x => x.GetAccessToken(true), Times.Never);
    }

    [Fact]
    public async Task Should_Retry_When_Health_Check_Failed()
    {
        // Arrange
        var listener = CreateTfexListener();
        var tokenSource = new CancellationTokenSource();
        var cancellationToken = tokenSource.Token;

        _mockSetTradeService.Setup(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SettradeStreamResponse
            {
                Hosts = ["wss://test.mqtt"],
                Token = "test-token"
            });

        _mockMqttClientFactory.Setup(x => x.Ping(It.IsAny<IMqttClient>(), It.IsAny<CancellationToken>())).Throws(new Exception("Health check Failed"));

        // Act
        await listener.StartAsync(cancellationToken);
        await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);

        // Assert
        _mockMqttClientFactory.Verify(x => x.Ping(It.IsAny<IMqttClient>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Should_Cancel_Health_Check()
    {
        // Arrange
        var listener = CreateTfexListener();
        var tokenSource = new CancellationTokenSource();
        var cancellationToken = tokenSource.Token;

        _mockSetTradeService.Setup(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SettradeStreamResponse
            {
                Hosts = ["wss://test.mqtt"],
                Token = "test-token"
            });

        _mockMqttClientFactory.Setup(x => x.Ping(It.IsAny<IMqttClient>(), It.IsAny<CancellationToken>())).Throws(new Exception("Health check Failed"));

        // Act
        await listener.StartAsync(cancellationToken);
        await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
        await listener.HealthCheckCancellationTokenSource!.CancelAsync();
        await Task.Delay(TimeSpan.FromMilliseconds(1100), cancellationToken);

        // Assert
        _mockMqttClientFactory.Verify(x => x.Ping(It.IsAny<IMqttClient>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Should_Waiting_For_Operating_Hours()
    {
        // Arrange
        _mockOperationHoursOptions.Setup(x => x.Value).Returns(new OperationHoursOptions
        {
            Start = "23:59:00",
            End = "23:59:01"
        });

        var listener = CreateTfexListener();
        var tokenSource = new CancellationTokenSource();
        var cancellationToken = tokenSource.Token;

        _mockSetTradeService.Setup(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SettradeStreamResponse
            {
                Hosts = ["wss://test.mqtt"],
                Token = "test-token"
            });

        // Act
        await listener.StartAsync(cancellationToken);

        // Assert
        _mockMqttClientFactory.Verify(x => x.Connect(It.IsAny<IMqttClient>(), It.IsAny<MqttClientOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Publish_Message_When_Received_Message()
    {
        // Arrange
        var listener = CreateTfexListener();

        var message = new OrderDerivV3
        {
            Version = 1,
            OrderNo = "1111",
            ExtOrderNo = "2222",
            AccountNo = "0123456789",
            EnterId = "3333",
            EntryTime = Timestamp.FromDateTime(DateTime.UtcNow),
            SeriesId = "4444",
            Side = OrderDerivV3.Types.LongShort.Long,
            Position = OrderDerivV3.Types.Position.Open,
            Price = new Money { CurrencyCode = "USD", Units = 10, Nanos = 100 },
            PriceType = OrderDerivV3.Types.PriceType.Limit,
            Volume = 100,
            BalanceVolume = 50,
            MatchedVolume = 30,
            CancelledVolume = 20,
            Valid = OrderDerivV3.Types.Valid.Day,
            Until = "Until",
            Status = "S",
            CanCancel = true,
            CanChange = true,
            CanChangePriceVol = true,
            IsTradeReport = true
        };

        var applicationMessage = new MqttApplicationMessage
        {
            PayloadSegment = message.ToByteArray()
        };

        var eventArgs = new MqttApplicationMessageReceivedEventArgs("clientId", applicationMessage, new MqttPublishPacket(), AcknowledgeHandler);

        // Act
        var handleListenerMessageReceivedMethod = typeof(TfexListener).GetMethod("HandleListenerMessageReceived", BindingFlags.NonPublic | BindingFlags.Instance);
        await (Task)handleListenerMessageReceivedMethod!.Invoke(listener, new object[] { eventArgs })!;

        // Assert
        _mockBus.Verify(bus => bus.Publish(It.IsAny<SetTradeOrderStatus>(), It.IsAny<CancellationToken>()), Times.Once);

        return;

        Task AcknowledgeHandler(MqttApplicationMessageReceivedEventArgs args, CancellationToken token) => Task.CompletedTask;
    }

    [Fact]
    public async Task Should_Connect()
    {
        // Arrange
        var listener = CreateTfexListener();

        var connectArgs = new MqttClientConnectedEventArgs(new MqttClientConnectResult());
        var cancellationToken = new CancellationTokenSource().Token;

        _mockSetTradeService.Setup(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SettradeStreamResponse
            {
                Hosts = ["wss://test.mqtt"],
                Token = "test-token"
            });

        await listener.StartAsync(cancellationToken);

        // Act
        var handleConnectedMethod = typeof(TfexListener).GetMethod("HandleListenerConnected", BindingFlags.NonPublic | BindingFlags.Instance);
        await (Task)handleConnectedMethod!.Invoke(listener, new object[] { connectArgs, cancellationToken })!;

        // Assert
        _mockMqttClientFactory.Verify(client => client.Subscribe(It.IsAny<IMqttClient>(), It.IsAny<MqttClientSubscribeOptions>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Should_Reconnect_When_Disconnect()
    {
        // Arrange
        _mockSetTradeStreamOptions.Setup(x => x.Value).Returns(new SetTradeStreamOptions
        {
            ConnectionDelaySeconds = 1,
        });

        var listener = CreateTfexListener();
        var tokenSource = new CancellationTokenSource();
        var cancellationToken = tokenSource.Token;

        var connectArgs = new MqttClientDisconnectedEventArgs(
            false,
            new MqttClientConnectResult(),
            MqttClientDisconnectReason.NormalDisconnection,
            "",
            [],
            new Exception("401 Unauthorized")
        );

        _mockSetTradeService.Setup(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SettradeStreamResponse
            {
                Hosts = ["wss://test.mqtt"],
                Token = "test-token"
            });

        // Act
        await listener.StartAsync(cancellationToken);
        var handleDisconnectedMethod = typeof(TfexListener).GetMethod("HandleListenerDisconnected", BindingFlags.NonPublic | BindingFlags.Instance);

        _ = Task.Run(async () =>
        {
            await (Task)handleDisconnectedMethod!.Invoke(listener, new object[] { connectArgs, cancellationToken })!;
        }, cancellationToken);

        await Task.Delay(TimeSpan.FromMilliseconds(1500), cancellationToken);
        await tokenSource.CancelAsync();

        // Assert
        _mockMqttClientFactory.Verify(x => x.Connect(It.IsAny<IMqttClient>(), It.IsAny<MqttClientOptions>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Should_Handle_Exception_When_Get_SetTrade_Url_Failed_During_Reconnect()
    {
        // Arrange
        _mockSetTradeStreamOptions.Setup(x => x.Value).Returns(new SetTradeStreamOptions
        {
            ConnectionDelaySeconds = 1,
        });

        var listener = CreateTfexListener();
        var tokenSource = new CancellationTokenSource();
        var cancellationToken = tokenSource.Token;

        var connectArgs = new MqttClientDisconnectedEventArgs(
            false,
            new MqttClientConnectResult(),
            MqttClientDisconnectReason.NormalDisconnection,
            "",
            [],
            null
        );

        _mockSetTradeService.Setup(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SettradeStreamResponse
            {
                Hosts = ["wss://test.mqtt"],
                Token = "test-token"
            });

        // Act
        await listener.StartAsync(cancellationToken);
        var handleDisconnectedMethod = typeof(TfexListener).GetMethod("HandleListenerDisconnected", BindingFlags.NonPublic | BindingFlags.Instance);

        _mockSetTradeService.Setup(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>())).Throws(new Exception("Failed To Get Url"));

        _ = Task.Run(async () =>
        {
            await (Task)handleDisconnectedMethod!.Invoke(listener, new object[] { connectArgs, cancellationToken })!;
        }, cancellationToken);

        await Task.Delay(TimeSpan.FromMilliseconds(1500), cancellationToken);
        await tokenSource.CancelAsync();

        // Assert
        _mockSetTradeService.Verify(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Should_Handle_Exception_When_Reconnect_Failed()
    {
        // Arrange
        _mockSetTradeStreamOptions.Setup(x => x.Value).Returns(new SetTradeStreamOptions
        {
            ConnectionDelaySeconds = 1,
        });

        var listener = CreateTfexListener();
        var tokenSource = new CancellationTokenSource();
        var cancellationToken = tokenSource.Token;

        var connectArgs = new MqttClientDisconnectedEventArgs(
            false,
            new MqttClientConnectResult(),
            MqttClientDisconnectReason.NormalDisconnection,
            "",
            [],
            null
        );

        _mockSetTradeService.Setup(x => x.GetSetTradeStreamInfo(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SettradeStreamResponse
            {
                Hosts = ["wss://test.mqtt"],
                Token = "test-token"
            });

        _mockMqttClientFactory
            .Setup(x => x.Connect(It.IsAny<IMqttClient>(), It.IsAny<MqttClientOptions>(),
                It.IsAny<CancellationToken>())).Throws(new Exception("Cannot connect"));

        // Act
        await listener.StartAsync(cancellationToken);
        var handleDisconnectedMethod = typeof(TfexListener).GetMethod("HandleListenerDisconnected", BindingFlags.NonPublic | BindingFlags.Instance);

        _ = Task.Run(async () =>
        {
            await (Task)handleDisconnectedMethod!.Invoke(listener, new object[] { connectArgs, cancellationToken })!;
        }, cancellationToken);

        await Task.Delay(TimeSpan.FromMilliseconds(1500), cancellationToken);
        await tokenSource.CancelAsync();

        // Assert
        _mockMqttClientFactory.Verify(x => x.Connect(It.IsAny<IMqttClient>(), It.IsAny<MqttClientOptions>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }
}