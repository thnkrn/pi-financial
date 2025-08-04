using Pi.SetMarketDataWSS.DataSubscriber.Models;
using Pi.SetMarketDataWSS.Domain.ConstantConfigurations;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Kafka;
using Polly;
using Polly.Retry;

namespace Pi.SetMarketDataWSS.DataSubscriber.Services;

public class KafkaSubscriptionService<TKey, TValue> : BackgroundService
{
    private readonly IKafkaSubscriber _kafkaSubscriber;
    private readonly ILogger<KafkaSubscriptionService<TKey, TValue>> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private bool _disposed;

    public KafkaSubscriptionService(
        IKafkaSubscriber kafkaSubscriber,
        IConfiguration configuration,
        ILogger<KafkaSubscriptionService<TKey, TValue>> logger)
    {
        _kafkaSubscriber = kafkaSubscriber ?? throw new ArgumentNullException(nameof(kafkaSubscriber));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        KafkaSubscriptionServiceConfig.MaxRetryAttempts = configuration.GetValue(
            ConfigurationKeys.KafkaSubscriptionServiceMaxRetryAttempts,
            KafkaSubscriptionServiceConfig.MaxRetryAttempts);
        KafkaSubscriptionServiceConfig.MaxRetryDelayMs = configuration.GetValue(
            ConfigurationKeys.KafkaSubscriptionServiceMaxRetryDelayMs, KafkaSubscriptionServiceConfig.MaxRetryDelayMs);
        KafkaSubscriptionServiceConfig.InitialRetryDelayMs = configuration.GetValue(
            ConfigurationKeys.KafkaSubscriptionServiceInitialRetryDelayMs,
            KafkaSubscriptionServiceConfig.InitialRetryDelayMs);

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                KafkaSubscriptionServiceConfig.MaxRetryAttempts,
                (attempt, _) =>
                    TimeSpan.FromMilliseconds(Math.Min(
                        KafkaSubscriptionServiceConfig.InitialRetryDelayMs * Math.Pow(2, attempt - 1),
                        KafkaSubscriptionServiceConfig.MaxRetryDelayMs)),
                (exception, timeSpan, retryCount, _) =>
                {
                    _logger.LogWarning(exception,
                        "Error occurred while subscribing to Kafka. Retrying in {Delay}ms. " +
                        "Attempt {RetryCount} of {MaxRetryAttempts}.",
                        timeSpan.TotalMilliseconds, retryCount, KafkaSubscriptionServiceConfig.MaxRetryAttempts);
                }
            );
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _retryPolicy.ExecuteAsync(async ct =>
            {
                _logger.LogDebug("Starting Kafka subscription service");
                await _kafkaSubscriber.SubscribeAsync(ct);
            }, stoppingToken);

            // If we get here, it means the subscription completed successfully
            _logger.LogDebug("Kafka subscription completed successfully");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Kafka subscription service was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unrecoverable error occurred in Kafka subscription service after all retry attempts");
            // Consider implementing a circuit breaker or alerting system here
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Stopping Kafka subscription service");
            await _kafkaSubscriber.UnsubscribeAsync();
            await base.StopAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while stopping Kafka subscription service");
        }
    }

    private void Disposing(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
            (_kafkaSubscriber as IDisposable)?.Dispose();

        _disposed = true;
    }

    public override void Dispose()
    {
        Disposing(true);
    }

    ~KafkaSubscriptionService()
    {
        Disposing(false);
    }
}