namespace Pi.SetMarketDataWSS.DataSubscriber.Models;

public static class KafkaSubscriptionServiceConfig
{
    public static int MaxRetryAttempts { get; set; } = 5;
    public static int InitialRetryDelayMs { get; set; } = 1000;
    public static int MaxRetryDelayMs { get; set; } = 30000;
}