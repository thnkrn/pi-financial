using System.Runtime.Serialization;

namespace Pi.TfexService.Domain.Exceptions;

[Serializable]
public class ItApiException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public ItApiException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public ItApiException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected ItApiException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class ItNotFoundException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public ItNotFoundException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public ItNotFoundException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected ItNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}