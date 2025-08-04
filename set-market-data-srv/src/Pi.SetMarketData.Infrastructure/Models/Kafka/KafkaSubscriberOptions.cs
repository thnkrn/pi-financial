namespace Pi.SetMarketData.Infrastructure.Models.Kafka;

public class KafkaSubscriberOptions
{
    public int SessionTimeoutMs { get; set; } = 45000;
    public int HeartbeatIntervalMs { get; set; } = 3000;
    public int MaxPollIntervalMs { get; set; } = 900000;
    public int SocketTimeoutMs { get; set; } = 60000;
    public int RetryIntervalSeconds { get; set; } = 5;
    public int TopicMetadataTimeoutSeconds { get; set; } = 10;
    public int ErrorRetryIntervalMs { get; set; } = 1000;
}