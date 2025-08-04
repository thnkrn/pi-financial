using NCrontab;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Pi.SetMarketData.SmartIntegration.Services;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.SmartIntegration.Interfaces;
using Pi.SetMarketData.SmartIntegration.Configurations;

namespace Pi.SetMarketData.SmartIntegration.Tests.Services;

public class CronJobServiceTests
{
    private readonly Mock<ILogger<CronJobService>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IDatabaseTaskService> _mockDatabaseTaskService;
    private readonly Mock<IConfigurationSection> _mockConfigSection;

    public CronJobServiceTests()
    {
        _mockLogger = new Mock<ILogger<CronJobService>>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfigSection = new Mock<IConfigurationSection>();
        _mockDatabaseTaskService = new Mock<IDatabaseTaskService>();
    }

    [Fact]
    public async Task ExecuteAsync_RunsOnSchedule()
    {
        // Arrange
        const string cronSchedule = "* * * * *"; // Run every minute
        SetupConfiguration(cronSchedule);

        // Setup logger with callback to track invocations
        var loggerInvoked = new TaskCompletionSource<bool>();
        _mockLogger
            .Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(() => loggerInvoked.SetResult(true));

        // Setup database task with callback
        var databaseTaskInvoked = new TaskCompletionSource<bool>();
        _mockDatabaseTaskService
            .Setup(x => x.PerformDatabaseTask(It.IsAny<BatchUpdateOptions>()))
            .Callback(() => databaseTaskInvoked.SetResult(true))
            .Returns(Task.CompletedTask);

        // Create service with a past next run time to trigger immediate execution
        var service = new CronJobService(
            _mockLogger.Object,
            _mockDatabaseTaskService.Object,
            new ApplicationLifetime(new Logger<ApplicationLifetime>(new LoggerFactory()))
        );

        // Use reflection to set _nextRun to a pastime to ensure immediate execution
        var nextRunField = typeof(CronJobService).GetField("_nextRun", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        nextRunField?.SetValue(service, DateTime.Now.AddMinutes(-1));

        // Act
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        // Start the service
        var executionTask = service.StartAsync(cts.Token);

        // Wait for both logger and database task to be invoked or timeout
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(4));
        var completedTask = await Task.WhenAny(
            Task.WhenAll(loggerInvoked.Task, databaseTaskInvoked.Task),
            timeoutTask
        );

        // Stop the service
        await service.StopAsync(CancellationToken.None);

        // Assert
        Assert.NotEqual(timeoutTask, completedTask);
        Assert.True(await loggerInvoked.Task);
        Assert.True(await databaseTaskInvoked.Task);

        _mockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);

        _mockDatabaseTaskService.Verify(
            x => x.PerformDatabaseTask(It.IsAny<BatchUpdateOptions>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public void Constructor_ThrowsException_WhenCronScheduleNotConfigured()
    {
        // Arrange
        SetupConfiguration(null);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new CronJobService(
            _mockLogger.Object,
            _mockDatabaseTaskService.Object,
            new ApplicationLifetime(new Logger<ApplicationLifetime>(new LoggerFactory()))
        ));
    }

    [Theory]
    [InlineData("invalid cron")]
    [InlineData("* * * *")] // Missing one field
    public void Constructor_ThrowsException_WhenCronExpressionIsInvalid(string cronExpression)
    {
        // Arrange
        SetupConfiguration(cronExpression);

        // Act & Assert
        Assert.Throws<CrontabException>(() => new CronJobService(
            _mockLogger.Object,
            _mockDatabaseTaskService.Object,
            new ApplicationLifetime(new Logger<ApplicationLifetime>(new LoggerFactory()))
        ));
    }

    private void SetupConfiguration(string? value)
    {
        var sections = ConfigurationKeys.CronjobSchedule.Split(':');

        if (sections.Length > 1)
        {
            // Setup for nested configuration
            var parentSection = new Mock<IConfigurationSection>();
            var childSection = new Mock<IConfigurationSection>();

            // Setup the parent section
            _mockConfiguration
                .Setup(x => x.GetSection(sections[0]))
                .Returns(parentSection.Object);

            // Setup parent section's value
            parentSection
                .Setup(x => x[sections[1]])
                .Returns(value);

            // Setup child section
            parentSection
                .Setup(x => x.GetSection(sections[1]))
                .Returns(childSection.Object);

            // Setup child section's value
            childSection
                .Setup(x => x.Value)
                .Returns(value);

            // Setup for complete path
            _mockConfiguration
                .Setup(x => x.GetSection(ConfigurationKeys.CronjobSchedule))
                .Returns(childSection.Object);
        }
        else
        {
            // Simple configuration without nesting
            _mockConfigSection
                .Setup(x => x.Value)
                .Returns(value);

            _mockConfiguration
                .Setup(x => x.GetSection(ConfigurationKeys.CronjobSchedule))
                .Returns(_mockConfigSection.Object);
        }

        // Setup direct access
        _mockConfiguration
            .Setup(x => x[ConfigurationKeys.CronjobSchedule])
            .Returns(value);
    }
}