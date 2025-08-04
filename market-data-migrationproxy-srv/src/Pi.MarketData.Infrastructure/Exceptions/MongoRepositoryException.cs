namespace Pi.MarketData.Infrastructure.Exceptions;

public class MongoRepositoryException : Exception
{
    public MongoRepositoryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}