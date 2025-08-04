namespace Pi.SetMarketData.DataProcessingService.Exceptions;

public class DataProcessingServiceException : Exception
{

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public DataProcessingServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}