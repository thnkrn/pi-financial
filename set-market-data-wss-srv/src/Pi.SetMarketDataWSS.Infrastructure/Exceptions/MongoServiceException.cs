namespace Pi.SetMarketDataWSS.Infrastructure.Exceptions;

public class MongoServiceException : Exception
{
    public MongoServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}