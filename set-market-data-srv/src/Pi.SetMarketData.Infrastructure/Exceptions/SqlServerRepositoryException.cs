namespace Pi.SetMarketData.Infrastructure.Exceptions;

public class SqlServerRepositoryException : Exception
{
    public SqlServerRepositoryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}