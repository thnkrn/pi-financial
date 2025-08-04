namespace Pi.SetMarketData.Infrastructure.Exceptions;

public class KafkaConfigurationException : Exception
{
    public KafkaConfigurationException(string message) : base(message) { }
    public KafkaConfigurationException(string message, Exception innerException) 
        : base(message, innerException) { }
}