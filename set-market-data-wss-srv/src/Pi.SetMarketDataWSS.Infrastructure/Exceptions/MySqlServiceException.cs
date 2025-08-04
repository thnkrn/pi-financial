namespace Pi.SetMarketDataWSS.Infrastructure.Exceptions;

public class MySqlServiceException : Exception
{
    public MySqlServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}