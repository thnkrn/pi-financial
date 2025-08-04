namespace Pi.SetMarketData.Infrastructure.Exceptions;

public class MySqlRepositoryException : Exception
{
    public MySqlRepositoryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}