namespace Pi.SetMarketDataRealTime.Infrastructure.Exceptions;

public class TimescaleRepositoryException : Exception
{
    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public TimescaleRepositoryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}