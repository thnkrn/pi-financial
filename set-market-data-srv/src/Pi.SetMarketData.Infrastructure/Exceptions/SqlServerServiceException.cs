namespace Pi.SetMarketData.Infrastructure.Exceptions;

public class SqlServerServiceException : Exception
{
    public SqlServerServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}