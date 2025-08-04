using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Pi.Common.Serilog;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Kafka;
using Serilog;
using Serilog.Debugging;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Kafka;

public sealed class KafkaSubscriber<TKey, TValue> : IKafkaSubscriber, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IKafkaMessageHandler<TValue> _messageHandler;
    private readonly string? _topic;
    private IConsumer<TKey, TValue>? _consumer;
    private bool _disposed;

    public KafkaSubscriber(
        IConfiguration configuration,
        string? topic,
        IKafkaMessageHandler<TValue> messageHandler
    )
    {
        _configuration = configuration;
        _topic = topic;
        _messageHandler = messageHandler;
        Log.Logger = PiSerilogConfiguration.CreateBootstrapLogger();
        SelfLog.Enable(Console.Error);
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
                if (!await IsTopicAvailableAsync(adminClient, cancellationToken)) continue;

                using (_consumer = new ConsumerBuilder<TKey, TValue>(config).Build())
                {
                    _consumer.Subscribe(_topic);
                    await ConsumeMessagesAsync(cancellationToken);
                }

                break;
            }
            catch (Exception ex)
            {
                await HandleSubscriptionErrorAsync(ex, cancellationToken);
            }
    }

    public Task UnsubscribeAsync()
    {
        _consumer?.Close();
        _consumer?.Dispose();
        return Task.CompletedTask;
    }

    private ConsumerConfig CreateConsumerConfig()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration[ConfigurationKeys.KafkaBootstrapServers],
            SessionTimeoutMs = 45000,
            GroupId = _configuration[ConfigurationKeys.KafkaConsumerGroupId],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        ConfigureSecuritySettings(config);
        LogConfigurationSettings(config);

        return config;
    }

    private void ConfigureSecuritySettings(ClientConfig config)
    {
        config.SecurityProtocol = ParseEnumSetting(ConfigurationKeys.KafkaSecurityProtocol,
            "SASL_SSL", SecurityProtocol.SaslSsl);
        config.SaslMechanism =
            ParseEnumSetting(ConfigurationKeys.KafkaSaslMechanism, "PLAIN", SaslMechanism.Plain);
        config.SaslUsername = _configuration[ConfigurationKeys.KafkaSaslUsername];
        config.SaslPassword = _configuration[ConfigurationKeys.KafkaSaslPassword];
    }

    private T ParseEnumSetting<T>(string key, string defaultValue, T fallbackValue) where T : struct, Enum
    {
        var value = _configuration[key] ?? defaultValue;
        return Enum.TryParse<T>(value.Replace("_", string.Empty), true, out var result) ? result : fallbackValue;
    }

    private static void LogConfigurationSettings(ConsumerConfig config)
    {
        Console.WriteLine($"BootstrapServers: {config.BootstrapServers}");
        Console.WriteLine($"GroupId: {config.GroupId}");
        Console.WriteLine($"SecurityProtocol: {config.SecurityProtocol}");
        Console.WriteLine($"SaslMechanism: {config.SaslMechanism}");
        Console.WriteLine($"SaslUsername: {config.SaslUsername}");
        Console.WriteLine($"SaslPassword: {config.SaslPassword}");
    }

    private async Task<bool> IsTopicAvailableAsync(IAdminClient adminClient, CancellationToken cancellationToken)
    {
        var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
        var topicMetadata = metadata.Topics.Find(t => t.Topic == _topic);

        if (topicMetadata == null)
        {
            Console.WriteLine($"Topic '{_topic}' does not exist. Retrying in 5 seconds...");
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            return false;
        }

        return true;
    }

    private async Task ConsumeMessagesAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
            try
            {
                var consumeResult = _consumer?.Consume(cancellationToken);
                if (consumeResult == null) continue;

                await _messageHandler.HandleAsync(consumeResult.Message.Value);
                _consumer?.Commit(consumeResult);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while consuming message: {ex.Message}");
                throw;
            }
    }

    private static async Task HandleSubscriptionErrorAsync(Exception ex, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Error occurred while checking topic existence or consuming messages: {ex.Message}");
        Console.WriteLine("Retrying in 5 seconds...");
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing) _consumer?.Dispose();

        _disposed = true;
    }

    ~KafkaSubscriber()
    {
        Dispose(false);
    }
}