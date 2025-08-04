namespace Pi.SetMarketDataRealTime.Infrastructure.Exceptions;

public class TimescaleServiceException : Exception
{
    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public TimescaleServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}