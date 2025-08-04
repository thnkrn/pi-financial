namespace Pi.GlobalMarketData.Infrastructure.Exceptions;

public class MongoRepositoryException : Exception
{
    public MongoRepositoryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}