namespace Pi.SetMarketDataWSS.Infrastructure.Exceptions;

public class CompressionException : Exception
{
    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public CompressionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}