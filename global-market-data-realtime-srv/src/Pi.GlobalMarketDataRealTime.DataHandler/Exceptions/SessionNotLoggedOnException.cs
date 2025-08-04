namespace Pi.GlobalMarketDataRealTime.DataHandler.Exceptions;

public class SessionNotLoggedOnException : Exception
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public SessionNotLoggedOnException(string message) : base(message) { }
}