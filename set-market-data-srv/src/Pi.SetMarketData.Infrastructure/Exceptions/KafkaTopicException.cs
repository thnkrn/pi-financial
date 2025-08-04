namespace Pi.SetMarketData.Infrastructure.Exceptions;

public class KafkaTopicException : Exception
{
    public KafkaTopicException(string message) : base(message) { }
    public KafkaTopicException(string message, Exception innerException) 
        : base(message, innerException) { }
}