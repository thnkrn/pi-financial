using Confluent.Kafka;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;
using Pi.GlobalMarketData.Infrastructure.Exceptions;
using Pi.GlobalMarketData.Infrastructure.Models.Kafka;

namespace Pi.GlobalMarketData.Infrastructure.Services.Kafka;

public sealed class KafkaSubscriber<TKey, TValue> : IKafkaSubscriber<TKey, TValue>, IDisposable
{
    private const int MaxConsecutiveErrors = 3;
    private readonly IConfiguration _configuration;
    private readonly Stopwatch _consumeTimer = new();
    private readonly ILogger<KafkaSubscriber<TKey, TValue>> _logger;
    private readonly IKafkaMessageHandler<TKey, TValue> _messageHandler;
    private readonly KafkaSubscriberOptions _options;
    private readonly List<string> _topic;
    private int _consecutiveErrorCount;
    private IConsumer<TKey, TValue>? _consumer;
    private volatile bool _disposed;

    public KafkaSubscriber(
        IConfiguration configuration,
        ILogger<KafkaSubscriber<TKey, TValue>> logger,
        List<string> topic,
        IKafkaMessageHandler<TKey, TValue> messageHandler)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _topic = topic ?? throw new ArgumentNullException(nameof(topic));
        _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        _options = new KafkaSubscriberOptions();

        ValidateConfiguration();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task SubscribeAsync(CancellationToken cancellationToken)
    {
        var config = CreateConsumerConfig();
        using var adminClient = new AdminClientBuilder(config).Build();

        while (!cancellationToken.IsCancellationRequested)
            try
            {
                if (!await EnsureTopicExistsAsync(adminClient, cancellationToken)) continue;

                await ConsumeMessagesAsync(config, cancellationToken);
                break;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error occurred in Kafka subscription loop");
                await HandleSubscriptionErrorAsync(cancellationToken);
            }
    }

    public async Task UnsubscribeAsync()
    {
        try
        {
            _consumer?.Unsubscribe();
            _consumer?.Close();
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during unsubscribe");
        }
        finally
        {
            _consumer?.Dispose();
            _consumer = null;
        }
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(_configuration[ConfigurationKeys.KafkaBootstrapServers]))
            throw new InvalidOperationException("Kafka bootstrap servers configuration is missing");
        if (string.IsNullOrEmpty(_configuration[ConfigurationKeys.KafkaConsumerGroupId]))
            throw new InvalidOperationException("Kafka consumer group ID configuration is missing");
    }

    private ConsumerConfig CreateConsumerConfig()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration[ConfigurationKeys.KafkaBootstrapServers],
            SessionTimeoutMs = _options.SessionTimeoutMs,
            GroupId = _configuration[ConfigurationKeys.KafkaConsumerGroupId],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            // Add heartbeat interval for better connection management
            HeartbeatIntervalMs = _options.HeartbeatIntervalMs,
            // Add timeout configurations
            MaxPollIntervalMs = _options.MaxPollIntervalMs,
            SocketTimeoutMs = _options.SocketTimeoutMs
        };

        try
        {
            ConfigureSecuritySettings(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to configure Kafka security settings");
            throw new KafkaConfigurationException("Failed to configure Kafka security settings", ex);
        }

        return config;
    }

    private void ConfigureSecuritySettings(ClientConfig config)
    {
        var securityProtocolString = _configuration[ConfigurationKeys.KafkaSecurityProtocol] ?? "SASL_SSL";
        config.SecurityProtocol = Enum.TryParse<SecurityProtocol>(securityProtocolString.Replace("_", string.Empty),
            true, out var securityProtocol)
            ? securityProtocol
            : SecurityProtocol.SaslSsl;

        var saslMechanismString = _configuration[ConfigurationKeys.KafkaSaslMechanism] ?? "PLAIN";
        config.SaslMechanism = Enum.TryParse<SaslMechanism>(saslMechanismString.Replace("_", string.Empty),
            true, out var saslMechanism)
            ? saslMechanism
            : SaslMechanism.Plain;

        config.SaslUsername = _configuration[ConfigurationKeys.KafkaSaslUsername];
        config.SaslPassword = _configuration[ConfigurationKeys.KafkaSaslPassword];

        if (string.IsNullOrEmpty(config.SaslUsername) || string.IsNullOrEmpty(config.SaslPassword))
            throw new KafkaConfigurationException("SASL credentials are not properly configured");
    }

    private async Task<bool> EnsureTopicExistsAsync(IAdminClient adminClient, CancellationToken cancellationToken)
    {
        try
        {
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(_options.TopicMetadataTimeoutSeconds));
            var topicMetadata = metadata.Topics.Find(t => _topic.Contains(t.Topic));

            if (topicMetadata == null)
            {
                _logger.LogWarning("Topic '{Topic}' does not exist. Retrying in {RetrySeconds} seconds...",
                    _topic, _options.RetryIntervalSeconds);
                await Task.Delay(TimeSpan.FromSeconds(_options.RetryIntervalSeconds), cancellationToken);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check topic existence");
            throw new KafkaTopicException($"Failed to verify topic '{_topic}'", ex);
        }
    }

    private async Task ConsumeMessagesAsync(ConsumerConfig config, CancellationToken cancellationToken)
    {
        _consumer = new ConsumerBuilder<TKey, TValue>(config)
            .SetErrorHandler((_, error) => _logger.LogError("Kafka error: {Error}", error.Reason))
            .SetStatisticsHandler((_, json) => _logger.LogTrace("Kafka statistics: {Statistics}", json))
            .Build();

        _consumer.Subscribe(_topic);
        _logger.LogInformation("Successfully subscribed to topic: {Topic}", _topic);

        while (!cancellationToken.IsCancellationRequested)
            try
            {
                _consumeTimer.Restart();
                var consumeResult = _consumer.Consume(cancellationToken);
                _consumeTimer.Stop();

                if (consumeResult == null) continue;

                LogMessageMetrics(consumeResult);
                await ProcessMessageAsync(consumeResult);
                _consecutiveErrorCount = 0;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                await HandleConsumptionErrorAsync(ex, cancellationToken);
            }
    }

    private async Task ProcessMessageAsync(ConsumeResult<TKey, TValue> consumeResult)
    {
        try
        {
            await _messageHandler.HandleAsync(consumeResult);
            _consumer?.Commit(consumeResult);
            _logger.LogInformation(
                "Successfully processed and committed message from topic {Topic}, partition {Partition}, offset {Offset}",
                consumeResult.Topic, consumeResult.Partition, consumeResult.Offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message from topic {Topic}, partition {Partition}, offset {Offset}",
                consumeResult.Topic, consumeResult.Partition, consumeResult.Offset);
            throw new KafkaTopicException("Error processing message from topic");
        }
    }

    private void LogMessageMetrics(ConsumeResult<TKey, TValue> consumeResult)
    {
        _logger.LogTrace(
            "Message consumed from {Topic}: Partition = {Partition}, Offset = {Offset}, Timestamp = {Timestamp}, ProcessingTime = {ProcessingTime}ms",
            consumeResult.Topic,
            consumeResult.Partition,
            consumeResult.Offset,
            consumeResult.Message.Timestamp.UtcDateTime,
            _consumeTimer.ElapsedMilliseconds);
    }

    private async Task HandleConsumptionErrorAsync(Exception ex, CancellationToken cancellationToken)
    {
        _consecutiveErrorCount++;
        _logger.LogError(ex, "Error consuming message. Consecutive error count: {ErrorCount}", _consecutiveErrorCount);

        if (_consecutiveErrorCount >= MaxConsecutiveErrors)
        {
            _logger.LogCritical("Max consecutive errors reached. Initiating consumer restart...");
            await RestartConsumerAsync();
        }
        else
        {
            await Task.Delay(_options.ErrorRetryIntervalMs, cancellationToken);
        }
    }

    private async Task HandleSubscriptionErrorAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("Retrying subscription in {RetrySeconds} seconds...", _options.RetryIntervalSeconds);
        await Task.Delay(TimeSpan.FromSeconds(_options.RetryIntervalSeconds), cancellationToken);
    }

    private async Task RestartConsumerAsync()
    {
        await UnsubscribeAsync();
        _consecutiveErrorCount = 0;
        _logger.LogInformation("Consumer has been reset and will attempt to reconnect");
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
            try
            {
                _consumer?.Unsubscribe();
                _consumer?.Close();
                _consumer?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during disposal");
            }

        _disposed = true;
    }

    ~KafkaSubscriber()
    {
        Dispose(false);
    }
}