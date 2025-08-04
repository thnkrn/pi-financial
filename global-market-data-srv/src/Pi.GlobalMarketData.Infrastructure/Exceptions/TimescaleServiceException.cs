namespace Pi.GlobalMarketData.Infrastructure.Exceptions;

public class TimescaleServiceException : Exception
{
    public TimescaleServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}