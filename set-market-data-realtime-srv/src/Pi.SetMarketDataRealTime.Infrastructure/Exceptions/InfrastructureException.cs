namespace Pi.SetMarketDataRealTime.Infrastructure.Exceptions;

public class InfrastructureException : Exception
{
    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public InfrastructureException(string message, Exception innerException) : base(message, innerException)
    {
    }
}