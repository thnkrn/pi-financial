namespace Pi.MarketData.Search.Infrastructure.Exceptions.Mongo;

public class MongoServiceException : Exception
{
    public MongoServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}