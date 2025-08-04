using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pi.SetMarketData.Infrastructure.Interfaces.Kafka;
using Polly;
using Polly.Retry;

namespace Pi.SetMarketData.Infrastructure.Services.Kafka;

public class KafkaSubscriptionService<TKey, TValue> : BackgroundService
{
    private const int MaxRetryAttempts = 5;
    private const int MaxRetryDelayMs = 1000;
    private const int InitialRetryDelayMs = 30000;
    private readonly IKafkaSubscriber<TKey, TValue> _kafkaSubscriber;
    private readonly ILogger<KafkaSubscriptionService<TKey, TValue>> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public KafkaSubscriptionService(
        IKafkaSubscriber<TKey, TValue> kafkaSubscriber,
        ILogger<KafkaSubscriptionService<TKey, TValue>> logger)
    {
        _kafkaSubscriber = kafkaSubscriber ?? throw new ArgumentNullException(nameof(kafkaSubscriber));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                MaxRetryAttempts,
                (attempt, _) =>
                    TimeSpan.FromMilliseconds(Math.Min(
                        InitialRetryDelayMs * Math.Pow(2, attempt - 1),
                        MaxRetryDelayMs)),
                (exception, timeSpan, retryCount, _) =>
                {
                    _logger.LogWarning(exception,
                        "Error occurred while subscribing to Kafka. Retrying in {Delay}ms. " +
                        "Attempt {RetryCount} of {MaxRetryAttempts}.",
                        timeSpan.TotalMilliseconds, retryCount, MaxRetryAttempts);
                }
            );
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _retryPolicy.ExecuteAsync(async ct =>
            {
                _logger.LogInformation("Starting Kafka subscription service");
                await _kafkaSubscriber.SubscribeAsync(ct);
            }, stoppingToken);

            // If we get here, it means the subscription completed successfully
            _logger.LogInformation("Kafka subscription completed successfully");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogInformation(ex, "Kafka subscription service was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unrecoverable error occurred in Kafka subscription service after all retry attempts");
            // You can add further actions like circuit breaker or alerting system here
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Stopping Kafka subscription service");
            await _kafkaSubscriber.UnsubscribeAsync();
            await base.StopAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while stopping Kafka subscription service");
        }
    }
}