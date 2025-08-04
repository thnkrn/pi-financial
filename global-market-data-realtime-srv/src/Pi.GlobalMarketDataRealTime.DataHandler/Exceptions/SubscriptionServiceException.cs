namespace Pi.GlobalMarketDataRealTime.DataHandler.Exceptions;

public class SubscriptionServiceException : Exception
{
    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public SubscriptionServiceException(string message, Exception? innerException = null) : base(message,
        innerException)
    {
    }
}