namespace Pi.GlobalMarketDataRealTime.Infrastructure.Exceptions;

public class MongoServiceException : Exception
{
    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public MongoServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}