namespace Pi.SetMarketData.Application.Exceptions;

public class BusinessHoursDeterminationException : Exception
{
    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public BusinessHoursDeterminationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}