using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Pi.SetMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.Kafka;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.Kafka;

public sealed class KafkaSubscriber<TKey, TValue> : IKafkaSubscriber<TKey, TValue>, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IKafkaMessageHandler<TValue> _messageHandler;
    private readonly string? _topic;
    private IConsumer<TKey, TValue>? _consumer;
    private bool _disposed;

    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="topic"></param>
    /// <param name="messageHandler"></param>
    public KafkaSubscriber(IConfiguration configuration, string? topic, IKafkaMessageHandler<TValue> messageHandler)
    {
        _configuration = configuration;
        _topic = topic;
        _messageHandler = messageHandler;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task SubscribeAsync(CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration[ConfigurationKeys.KafkaBootstrapServers],
            SessionTimeoutMs = 45000,
            GroupId = _configuration[ConfigurationKeys.KafkaConsumerGroupId],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        Console.WriteLine($"BootstrapServers: {_configuration[ConfigurationKeys.KafkaBootstrapServers]}");
        Console.WriteLine($"GroupId: {_configuration[ConfigurationKeys.KafkaConsumerGroupId]}");

        try
        {
            var securityProtocolString = _configuration[ConfigurationKeys.KafkaSecurityProtocol] ?? "SASL_SSL";
            if (Enum.TryParse<SecurityProtocol>(securityProtocolString.Replace("_", string.Empty), true,
                    out var securityProtocol))
            {
                config.SecurityProtocol = securityProtocol;
                Console.WriteLine($"SecurityProtocol: {securityProtocol}");
            }
            else
            {
                Console.WriteLine($"Invalid SecurityProtocol value: {securityProtocolString}. Using default SASL_SSL.");
                config.SecurityProtocol = SecurityProtocol.SaslSsl;
            }

            var saslMechanismString = _configuration[ConfigurationKeys.KafkaSaslMechanism] ?? "PLAIN";
            if (Enum.TryParse<SaslMechanism>(saslMechanismString.Replace("_", string.Empty), true,
                    out var saslMechanism))
            {
                config.SaslMechanism = saslMechanism;
                Console.WriteLine($"SaslMechanism: {saslMechanism}");
            }
            else
            {
                Console.WriteLine($"Invalid SaslMechanism value: {saslMechanismString}. Using default PLAIN.");
                config.SaslMechanism = SaslMechanism.Plain;
            }

            config.SaslUsername = _configuration[ConfigurationKeys.KafkaSaslUsername];
            Console.WriteLine($"SaslUsername: {_configuration[ConfigurationKeys.KafkaSaslUsername]}");

            config.SaslPassword = _configuration[ConfigurationKeys.KafkaSaslPassword];
            Console.WriteLine($"SaslPassword: {_configuration[ConfigurationKeys.KafkaSaslPassword]}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error configuring Kafka consumer: {ex.Message}");
            throw;
        }

        using var adminClient = new AdminClientBuilder(config).Build();
        while (!cancellationToken.IsCancellationRequested)
            try
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                var topicMetadata = metadata.Topics.FirstOrDefault(t => t.Topic == _topic);

                if (topicMetadata == null)
                {
                    Console.WriteLine($"Topic '{_topic}' does not exist. Retrying in 5 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    continue;
                }

                _consumer = new ConsumerBuilder<TKey, TValue>(config).Build();
                _consumer.Subscribe(_topic);

                while (!cancellationToken.IsCancellationRequested)
                    try
                    {
                        var consumeResult = _consumer.Consume(cancellationToken);
                        if (consumeResult == null) continue;

                        await _messageHandler.HandleAsync(consumeResult.Message.Value);

                        // Manually commit the offset
                        _consumer.Commit(consumeResult);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }

                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error occurred while checking topic existence or consuming messages: {ex.Message}");
                Console.WriteLine("Retrying in 5 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
    }

    public Task UnsubscribeAsync()
    {
        _consumer?.Close();
        _consumer?.Dispose();
        return Task.CompletedTask;
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing) _consumer?.Dispose();

        _disposed = true;
    }

    ~KafkaSubscriber()
    {
        Dispose(false);
    }
}