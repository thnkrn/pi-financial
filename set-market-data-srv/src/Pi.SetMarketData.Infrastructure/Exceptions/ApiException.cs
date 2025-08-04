namespace Pi.SetMarketData.Infrastructure.Exceptions;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public ApiException(string message, int statusCode = 500) 
        : base(message)
    {
        StatusCode = statusCode;
    }
}