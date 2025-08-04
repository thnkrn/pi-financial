using System.Net;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Interfaces.MessageValidator;
using Pi.SetMarketDataRealTime.DataHandler.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.DataHandler.Services.SoupBinTcp;
using Pi.SetMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.AmazonS3;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp;

namespace Pi.SetMarketDataRealTime.DataHandler.Tests;

public class ClientSubscriptionServiceAutoRestartTests
{
    private readonly ClientSubscriptionDependencies _dependencies;
    private readonly Mock<IClientFactory> _mockClientFactory;
    private readonly Mock<IClientListener> _mockClientListener;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<ClientSubscriptionServiceAutoRestart>> _mockLogger;
    private readonly Mock<IMemoryCacheHelper> _mockMemoryCacheHelper;
    private readonly Mock<IMessageValidator> _mockMessageValidator;
    private readonly Mock<IAmazonS3Service> _mockS3Service;
    private readonly Mock<IServiceProvider> _mockServiceProvider;

    public ClientSubscriptionServiceAutoRestartTests()
    {
        _mockClientFactory = new Mock<IClientFactory>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockClientListener = new Mock<IClientListener>();
        _mockMessageValidator = new Mock<IMessageValidator>();
        _mockLogger = new Mock<ILogger<ClientSubscriptionServiceAutoRestart>>();
        _mockMemoryCacheHelper = new Mock<IMemoryCacheHelper>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockS3Service = new Mock<IAmazonS3Service>();

        _dependencies = new ClientSubscriptionDependencies(
            _mockClientFactory.Object,
            _mockClientListener.Object,
            _mockMessageValidator.Object,
            _mockMemoryCacheHelper.Object,
            _mockS3Service.Object
        );

        // Setup mock configuration
        SetupMockConfiguration();
    }

    private void SetupMockConfiguration()
    {
        var configurationValues = new Dictionary<string, string>
        {
            [ConfigurationKeys.SettradeRunLocalMode] = "false",
            [ConfigurationKeys.SettradeHolidayApiIsActivated] = "true",
            [ConfigurationKeys.SettradeStockMarketStartCronJob] = "0 8 * * 1-5",
            [ConfigurationKeys.SettradeStockMarketStopCronJob] = "0 19 * * 1-5",
            [ConfigurationKeys.SettradeClientConfigIpAddress] = "127.0.0.1",
            [ConfigurationKeys.SettradeClientConfigPort] = "8080",
            [ConfigurationKeys.SettradeClientConfigReconnectDelayMs] = "1000",
            [ConfigurationKeys.SettradeGatewaySettingsReconnectDelayMs] = "1000"
        };

        foreach (var kvp in configurationValues)
        {
            var sectionMock = new Mock<IConfigurationSection>();
            sectionMock.Setup(s => s.Value).Returns(kvp.Value);
            _mockConfiguration.Setup(x => x.GetSection(kvp.Key)).Returns(sectionMock.Object);
        }

        // Setup for SettradeGatewaySettings section
        var settradeSectionMock = new Mock<IConfigurationSection>();
        _mockConfiguration.Setup(x => x.GetSection(ConfigurationKeys.SettradeGatewaySettings))
            .Returns(settradeSectionMock.Object);
    }

    [Fact]
    public async Task HandleUnexpectedDisconnectionAsync_ShouldStopAndStartSubscriptionTask()
    {
        // Arrange
        var service = new ClientSubscriptionServiceAutoRestart(
            _dependencies,
            _mockConfiguration.Object,
            _mockLogger.Object,
            _mockServiceProvider.Object
        );

        // Act
        await service.HandleUnexpectedDisconnectionAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Handling unexpected disconnection")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task StartAsync_ShouldInitializeAndStartExecution()
    {
        // Arrange
        var mockService = new Mock<ClientSubscriptionServiceAutoRestart>(
            _dependencies,
            _mockConfiguration.Object,
            _mockLogger.Object,
            _mockServiceProvider.Object
        ) { CallBase = true };

        mockService.Protected()
            .Setup<Task>("ExecuteAsync", ItExpr.IsAny<CancellationToken>())
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await mockService.Object.StartAsync(CancellationToken.None);

        // Assert
        mockService.Protected().Verify("ExecuteAsync", Times.Once(), ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task StartSubscriptionTask_ShouldAttemptToStartClient()
    {
        // Arrange
        var mockSettradeGatewaySettings = new Mock<IConfigurationSection>();
        mockSettradeGatewaySettings.Setup(x => x["SERVERS"]).Returns(
            "[{\"NAME\":\"TestServer\",\"GLIMPSE_GATEWAY\":{\"IP_ADDRESS\":\"127.0.0.1\",\"PORT\":8080},\"ITCH_GATEWAY\":{\"IP_ADDRESS\":\"127.0.0.1\",\"PORT\":8081}}]");

        _mockConfiguration.Setup(x => x.GetSection(ConfigurationKeys.SettradeGatewaySettings))
            .Returns(mockSettradeGatewaySettings.Object);
        _mockConfiguration.Setup(x => x[ConfigurationKeys.SettradeRunLocalMode]).Returns("false");
        _mockConfiguration.Setup(x => x[ConfigurationKeys.SettradeGatewaySettingsReconnectDelayMs]).Returns("1000");

        var service = new ClientSubscriptionServiceAutoRestart(
            _dependencies,
            _mockConfiguration.Object,
            _mockLogger.Object,
            _mockServiceProvider.Object
        );

        // Use reflection to access and set private fields
        var runningField = typeof(ClientSubscriptionServiceAutoRestart).GetField("_isSubscriptionRunning",
            BindingFlags.NonPublic | BindingFlags.Instance);
        runningField.SetValue(service, false);

        // Use reflection to access private method
        var startSubscriptionTaskMethod = typeof(ClientSubscriptionServiceAutoRestart).GetMethod(
            "StartSubscriptionTask",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        await (Task)startSubscriptionTaskMethod.Invoke(service, null);

        // Allow some time for the task to start and attempt client connection
        await Task.Delay(500);

        // Assert
        Assert.True((bool)runningField.GetValue(service));

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Starting client (Attempt 1/10)")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o.ToString().Contains("An unrecoverable error occurred while starting the client")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.AtLeastOnce
        );
    }

    [Fact]
    public async Task StopSubscriptionTask_ShouldStopClientWhenRunning()
    {
        // Arrange
        var mockClient = new Mock<IClient>();
        _mockClientFactory.Setup(x => x.CreateClient()).Returns(mockClient.Object);

        var service = new ClientSubscriptionServiceAutoRestart(
            _dependencies,
            _mockConfiguration.Object,
            _mockLogger.Object,
            _mockServiceProvider.Object
        );

        // Use reflection to access and set private fields
        var runningField = typeof(ClientSubscriptionServiceAutoRestart).GetField("_isSubscriptionRunning",
            BindingFlags.NonPublic | BindingFlags.Instance);
        runningField.SetValue(service, true);

        var clientRunningField = typeof(ClientSubscriptionServiceAutoRestart).GetField("_isClientRunning",
            BindingFlags.NonPublic | BindingFlags.Instance);
        clientRunningField.SetValue(service, true);

        var clientCtsField =
            typeof(ClientSubscriptionServiceAutoRestart).GetField("_clientCts",
                BindingFlags.NonPublic | BindingFlags.Instance);
        var originalCts = (CancellationTokenSource)clientCtsField.GetValue(service);

        // Simulate a running task
        var simulatedTask = Task.Run(async () =>
        {
            while (!originalCts.Token.IsCancellationRequested) await Task.Delay(100);
        });

        // Use reflection to access private method
        var stopSubscriptionTaskMethod =
            typeof(ClientSubscriptionServiceAutoRestart).GetMethod("StopSubscriptionTask",
                BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        await (Task)stopSubscriptionTaskMethod.Invoke(service, null);

        // Assert
        _mockS3Service.Verify(x => x.UploadBinLogToS3Async(It.IsAny<string>()), Times.Once);
        Assert.False((bool)runningField.GetValue(service));
        Assert.False((bool)clientRunningField.GetValue(service));

        // Clean up
        await simulatedTask;
    }
}

// FakeClient class remains unchanged

internal class FakeClient : IClient
{
    private readonly ILogger _logger;

    public FakeClient(ILogger logger)
    {
        _logger = logger;
        State = ClientState.Disconnected;
    }

    public bool SetupAsyncCalled { get; private set; }
    public bool StartAsyncCalled { get; private set; }
    public ClientState State { get; private set; }

    public Task SetupAsync(IPAddress ipAddress, int port, int reconnectDelayMs, LoginDetails loginDetails)
    {
        _logger.LogDebug("FakeClient: SetupAsync called with IP: {IpAddress}, Port: {Port}", ipAddress, port);
        SetupAsyncCalled = true;
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("FakeClient: StartAsync called");
        StartAsyncCalled = true;
        State = ClientState.Connected;
        return Task.CompletedTask;
    }

    public Task ShutdownAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("ShutdownAsync called");
        State = ClientState.ShuttingDown;
        return Task.CompletedTask;
    }

    public Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("ConnectAsync called");
        State = ClientState.Connecting;
        return Task.CompletedTask;
    }

    public Task SendAsync(byte[]? message, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("SendAsync called");
        return Task.CompletedTask;
    }

    public Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("LogoutAsync called");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _logger.LogDebug("Dispose called");
        State = ClientState.Disconnected;
    }
}