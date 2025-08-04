namespace Pi.SetMarketDataRealTime.Infrastructure.Exceptions;

public class MySqlRepositoryException : Exception
{
    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public MySqlRepositoryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}