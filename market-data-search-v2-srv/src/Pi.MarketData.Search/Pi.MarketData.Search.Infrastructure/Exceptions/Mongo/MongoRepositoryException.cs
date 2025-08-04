namespace Pi.MarketData.Search.Infrastructure.Exceptions.Mongo;

public class MongoRepositoryException : Exception
{
    public MongoRepositoryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}