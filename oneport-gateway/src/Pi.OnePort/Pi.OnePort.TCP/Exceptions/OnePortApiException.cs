namespace Pi.OnePort.TCP.Exceptions;

public class OnePortApiException : Exception
{
    public OnePortApiException(string? message) : base(message)
    {
    }
}
