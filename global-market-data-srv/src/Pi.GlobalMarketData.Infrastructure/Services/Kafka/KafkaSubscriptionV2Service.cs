using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;
using Polly;
using Polly.Retry;

namespace Pi.GlobalMarketData.Infrastructure.Services.Kafka;

public sealed class KafkaSubscriptionV2Service<TKey, TValue> : BackgroundService
{
    private readonly IHealthCheckReporter? _healthReporter;
    private readonly IKafkaV2Subscriber _kafkaSubscriber;
    private readonly ILogger<KafkaSubscriptionV2Service<TKey, TValue>> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly string _serviceName;
    private readonly Stopwatch _uptimeStopwatch = Stopwatch.StartNew();
    private bool _disposed;

    public KafkaSubscriptionV2Service(
        IKafkaV2Subscriber kafkaSubscriber,
        ILogger<KafkaSubscriptionV2Service<TKey, TValue>> logger,
        IHealthCheckReporter? healthReporter = null)
    {
        _kafkaSubscriber = kafkaSubscriber ?? throw new ArgumentNullException(nameof(kafkaSubscriber));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _healthReporter = healthReporter ?? throw new ArgumentNullException(nameof(healthReporter));
        _serviceName = typeof(TKey).Name + "-" + typeof(TValue).Name;

        const int maxRetryAttempts = 3;
        const int maxRetryDelayMs = 1000;
        const int initialRetryDelayMs = 30000;

        // Use a more sophisticated retry policy with jitter to avoid thundering herd issues
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                maxRetryAttempts,
                (attempt, _) =>
                {
                    // Calculate delay with exponential backoff and jitter
                    var baseDelay = initialRetryDelayMs * Math.Pow(2, attempt - 1);
                    var jitter = new Random().Next(
                        (int)(baseDelay * 0.8),
                        (int)(baseDelay * 1.2));

                    return TimeSpan.FromMilliseconds(Math.Min(jitter, maxRetryDelayMs));
                },
                (exception, timeSpan, retryCount, _) =>
                {
                    _logger.LogWarning(exception,
                        "Error occurred while subscribing to Kafka. Retrying in {Delay}ms. " +
                        "Attempt {RetryCount} of {MaxRetryAttempts}.",
                        timeSpan.TotalMilliseconds, retryCount, maxRetryAttempts);

                    _healthReporter?.ReportUnhealthy(_serviceName, $"Retrying Kafka connection. Attempt {retryCount}");
                }
            );

        _logger.LogInformation(
            "Kafka Subscription Service initialized with MaxRetryAttempts={MaxRetries}, " +
            "InitialRetryDelayMs={InitialDelay}ms, MaxRetryDelayMs={MaxDelay}ms",
            maxRetryAttempts,
            initialRetryDelayMs,
            maxRetryDelayMs);
    }

    public override void Dispose()
    {
        DisposeManagedResources(true);
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _healthReporter?.ReportStarting(_serviceName);

        _logger.LogInformation("Starting Kafka subscription service");
        var executionStopwatch = Stopwatch.StartNew();
        var restartCount = 0;

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await _retryPolicy.ExecuteAsync(async ct =>
                {
                    _logger.LogDebug("Starting Kafka subscription service");
                    _healthReporter?.ReportHealthy(_serviceName);

                    await _kafkaSubscriber.SubscribeAsync(ct);
                }, stoppingToken);

                // If we get here, it means the subscription completed successfully
                _logger.LogDebug("Kafka subscription completed successfully");
                break;
            }
            catch (OperationCanceledException opCancelEx)
            {
                _logger.LogWarning(opCancelEx, "Kafka subscription service was cancelled");
                _healthReporter?.ReportStopping(_serviceName, "Service was cancelled");
                break;
            }
            catch (Exception ex)
            {
                restartCount++;

                _logger.LogError(ex,
                    "Unrecoverable error occurred in Kafka subscription service after all retry attempts. Restart count: {RestartCount}",
                    restartCount);

                _healthReporter?.ReportUnhealthy(_serviceName,
                    $"Unrecoverable error after all retry attempts. Restart count: {restartCount}");

                // Implement circuit breaker pattern
                if (restartCount >= 5)
                {
                    _logger.LogCritical(
                        "Kafka subscription service has failed {RestartCount} times consecutively. " +
                        "Circuit breaking to prevent further resource exhaustion.",
                        restartCount);

                    _healthReporter?.ReportCritical(_serviceName,
                        $"Service is circuit breaking after {restartCount} consecutive failures");

                    // Wait longer between restarts as failures accumulate
                    await Task.Delay(Math.Min(10000 * restartCount, 60000), stoppingToken);
                }
                else
                {
                    // Wait before attempting restart
                    await Task.Delay(5000, stoppingToken);
                }
            }

        executionStopwatch.Stop();
        _logger.LogInformation(
            "Kafka subscription service execution completed after {Uptime}, with {RestartCount} restarts",
            executionStopwatch.Elapsed.ToString(@"d\.hh\:mm\:ss"),
            restartCount);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Kafka subscription service after running for {Uptime}",
            _uptimeStopwatch.Elapsed.ToString(@"d\.hh\:mm\:ss"));

        _healthReporter?.ReportStopping(_serviceName, "Service is stopping");

        try
        {
            var stopwatch = Stopwatch.StartNew();
            await _kafkaSubscriber.UnsubscribeAsync();
            stopwatch.Stop();

            _logger.LogInformation("Unsubscribed from Kafka topic in {ElapsedMs}ms",
                stopwatch.ElapsedMilliseconds);

            await base.StopAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while stopping Kafka subscription service");

            _healthReporter?.ReportUnhealthy(_serviceName, "Error during service shutdown");
        }
    }

    private void DisposeManagedResources(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Dispose managed resources
            (_kafkaSubscriber as IDisposable)?.Dispose();

            _healthReporter?.ReportStopped(_serviceName);
        }

        _disposed = true;
    }

    ~KafkaSubscriptionV2Service()
    {
        DisposeManagedResources(false);
    }
}

/// <summary>
///     Interface for reporting health status of services
/// </summary>
public interface IHealthCheckReporter
{
    void ReportStarting(string serviceName);
    void ReportHealthy(string serviceName);
    void ReportUnhealthy(string serviceName, string reason);
    void ReportStopping(string serviceName, string reason);
    void ReportStopped(string serviceName);
    void ReportCritical(string serviceName, string reason);
}

/// <summary>
///     Default implementation of health check reporter that logs to the provided logger
/// </summary>
public class LoggingHealthCheckReporter : IHealthCheckReporter
{
    private readonly ILogger<LoggingHealthCheckReporter> _logger;

    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    public LoggingHealthCheckReporter(ILogger<LoggingHealthCheckReporter> logger)
    {
        _logger = logger;
    }

    public void ReportStarting(string serviceName)
    {
        _logger.LogInformation("Service {ServiceName} is starting", serviceName);
    }

    public void ReportHealthy(string serviceName)
    {
        _logger.LogInformation("Service {ServiceName} is healthy", serviceName);
    }

    public void ReportUnhealthy(string serviceName, string reason)
    {
        _logger.LogWarning("Service {ServiceName} is unhealthy: {Reason}", serviceName, reason);
    }

    public void ReportStopping(string serviceName, string reason)
    {
        _logger.LogInformation("Service {ServiceName} is stopping: {Reason}", serviceName, reason);
    }

    public void ReportStopped(string serviceName)
    {
        _logger.LogInformation("Service {ServiceName} has stopped", serviceName);
    }

    public void ReportCritical(string serviceName, string reason)
    {
        _logger.LogCritical("Service {ServiceName} is in critical state: {Reason}", serviceName, reason);
    }
}