namespace Pi.GlobalMarketDataRealTime.Infrastructure.Exceptions;

public class InfrastructureServiceException : Exception
{
    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public InfrastructureServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}