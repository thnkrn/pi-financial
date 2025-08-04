namespace Pi.SetMarketDataRealTime.Infrastructure.Exceptions;

public class MySqlServiceException : Exception
{
    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public MySqlServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}