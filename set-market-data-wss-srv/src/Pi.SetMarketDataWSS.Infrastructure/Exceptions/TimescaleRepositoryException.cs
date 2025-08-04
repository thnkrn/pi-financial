namespace Pi.SetMarketDataWSS.Infrastructure.Exceptions;

public class TimescaleRepositoryException : Exception
{
    public TimescaleRepositoryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}