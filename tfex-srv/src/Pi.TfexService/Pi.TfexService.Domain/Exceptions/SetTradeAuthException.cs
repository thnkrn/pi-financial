using System.Runtime.Serialization;

namespace Pi.TfexService.Domain.Exceptions;

[Serializable]
public class SetTradeAuthException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public SetTradeAuthException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public SetTradeAuthException()
    {
    }

    public SetTradeAuthException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected SetTradeAuthException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class SetTradeRefreshTokenException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }

    public SetTradeRefreshTokenException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public SetTradeRefreshTokenException()
    {
    }

    public SetTradeRefreshTokenException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected SetTradeRefreshTokenException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}