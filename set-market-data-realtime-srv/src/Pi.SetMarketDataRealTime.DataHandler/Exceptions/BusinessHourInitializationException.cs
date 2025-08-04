namespace Pi.SetMarketDataRealTime.DataHandler.Exceptions;

public class BusinessHourInitializationException : Exception
{
    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public BusinessHourInitializationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}