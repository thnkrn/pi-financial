namespace Pi.GlobalMarketDataRealTime.Infrastructure.Exceptions;

public class MongoRepositoryException : Exception
{
    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public MongoRepositoryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}